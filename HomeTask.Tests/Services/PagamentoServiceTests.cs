using HomeTask.Application.Dtos;
using HomeTask.Application.Services;
using HomeTask.Domain.Enums;
using HomeTask.Tests.Helpers;

namespace HomeTask.Tests.Services;

public class PagamentoServiceTests
{
    [Fact]
    public async Task IniciarCheckoutAsync_QuandoAgendamentoNaoEstaAguardandoPagamento_DeveFalhar()
    {
        var agendamentoRepository = new FakeAgendamentoRepository();
        var pagamentoRepository = new FakePagamentoRepository();
        var prestadorRepository = new FakePrestadorRepository();
        var gateway = new FakePagamentoGateway();
        var agendamento = EntidadeFactory.CriarAgendamento(status: StatusAgendamento.EmAndamento);
        agendamentoRepository.Seed(agendamento);

        var service = new PagamentoService(pagamentoRepository, agendamentoRepository, prestadorRepository, gateway);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.IniciarCheckoutAsync(agendamento.Id, agendamento.ClienteId));

        Assert.Equal("Agendamento nao esta aguardando pagamento", ex.Message);
    }

    [Fact]
    public async Task ProcessarWebhookAsync_QuandoPagamentoAprovado_DeveConcluirAgendamentoEIncrementarPrestador()
    {
        var clienteUsuario = EntidadeFactory.CriarUsuario(tipo: TipoUsuario.Cliente);
        var cliente = EntidadeFactory.CriarCliente(id: Guid.NewGuid(), usuarioId: clienteUsuario.Id);
        EntidadeFactory.DefinirNavegacao(cliente, nameof(cliente.Usuario), clienteUsuario);

        var prestadorUsuario = EntidadeFactory.CriarUsuario(tipo: TipoUsuario.Prestador);
        var prestador = EntidadeFactory.CriarPrestador(usuarioId: prestadorUsuario.Id);
        var agendamento = EntidadeFactory.CriarAgendamento(
            clienteId: cliente.Id,
            prestadorId: prestador.Id,
            status: StatusAgendamento.AguardandoPagamento);
        EntidadeFactory.DefinirNavegacao(agendamento, nameof(agendamento.Cliente), cliente);

        var pagamento = EntidadeFactory.CriarPagamento(agendamentoId: agendamento.Id, status: StatusPagamento.Processando);

        var agendamentoRepository = new FakeAgendamentoRepository();
        agendamentoRepository.Seed(agendamento);

        var pagamentoRepository = new FakePagamentoRepository();
        pagamentoRepository.Seed(pagamento);

        var prestadorRepository = new FakePrestadorRepository();
        prestadorRepository.Seed(prestador);

        var gateway = new FakePagamentoGateway
        {
            StatusResponse = new PagamentoStatusGatewayDto
            {
                PagamentoExternoId = "mp-1",
                ReferenciaInterna = pagamento.Id.ToString(),
                Status = StatusPagamento.Aprovado,
                StatusExterno = "approved",
                PayloadExterno = "{}"
            }
        };

        var service = new PagamentoService(pagamentoRepository, agendamentoRepository, prestadorRepository, gateway);

        var resultado = await service.ProcessarWebhookAsync(new PagamentoWebhookDto
        {
            Topico = "payment",
            PagamentoExternoId = "mp-1"
        });

        Assert.NotNull(resultado);
        Assert.Equal(StatusPagamento.Aprovado, resultado!.Status);
        Assert.Equal(StatusAgendamento.Concluido, agendamento.Status);
        Assert.Equal(1, prestador.TotalServicosConcluidos);
    }

    [Fact]
    public async Task ProcessarWebhookAsync_QuandoDuplicado_DeveSerIdempotente()
    {
        var prestador = EntidadeFactory.CriarPrestador();
        var agendamento = EntidadeFactory.CriarAgendamento(prestadorId: prestador.Id, status: StatusAgendamento.Concluido);
        var pagamento = EntidadeFactory.CriarPagamento(agendamentoId: agendamento.Id, status: StatusPagamento.Aprovado);

        var agendamentoRepository = new FakeAgendamentoRepository();
        agendamentoRepository.Seed(agendamento);

        var pagamentoRepository = new FakePagamentoRepository();
        pagamentoRepository.Seed(pagamento);

        var prestadorRepository = new FakePrestadorRepository();
        prestador.IncrementarTotalServicosConcluidos();
        prestadorRepository.Seed(prestador);

        var gateway = new FakePagamentoGateway
        {
            StatusResponse = new PagamentoStatusGatewayDto
            {
                PagamentoExternoId = "mp-duplicado",
                ReferenciaInterna = pagamento.Id.ToString(),
                Status = StatusPagamento.Aprovado,
                StatusExterno = "approved",
                PayloadExterno = "{}"
            }
        };

        var service = new PagamentoService(pagamentoRepository, agendamentoRepository, prestadorRepository, gateway);

        await service.ProcessarWebhookAsync(new PagamentoWebhookDto
        {
            Topico = "payment",
            PagamentoExternoId = "mp-duplicado"
        });

        Assert.Equal(1, prestador.TotalServicosConcluidos);
    }

    [Fact]
    public async Task IniciarCheckoutAsync_QuandoValorFinalDoAgendamentoMudar_DeveSincronizarPagamentoEEnviarValorAtualizado()
    {
        var clienteUsuario = EntidadeFactory.CriarUsuario(tipo: TipoUsuario.Cliente);
        var cliente = EntidadeFactory.CriarCliente(id: Guid.NewGuid(), usuarioId: clienteUsuario.Id);
        EntidadeFactory.DefinirNavegacao(cliente, nameof(cliente.Usuario), clienteUsuario);

        var agendamento = EntidadeFactory.CriarAgendamento(clienteId: cliente.Id, status: StatusAgendamento.AguardandoPagamento);
        agendamento.DefinirDados(
            agendamento.Id,
            agendamento.ClienteId,
            agendamento.PrestadorId,
            agendamento.PrincipalServicoPrestadorId,
            agendamento.DataHoraAgendada,
            agendamento.DuracaoMinutos,
            agendamento.Status,
            agendamento.EnderecoId,
            agendamento.Observacoes,
            300,
            agendamento.DataSolicitacao,
            agendamento.DataResposta,
            agendamento.DataInicio,
            agendamento.DataConclusao,
            agendamento.MotivoRecusa);
        EntidadeFactory.DefinirNavegacao(agendamento, nameof(agendamento.Cliente), cliente);

        var pagamento = EntidadeFactory.CriarPagamento(agendamentoId: agendamento.Id, status: StatusPagamento.Recusado, valor: 100);

        var agendamentoRepository = new FakeAgendamentoRepository();
        agendamentoRepository.Seed(agendamento);

        var pagamentoRepository = new FakePagamentoRepository();
        pagamentoRepository.Seed(pagamento);

        var prestadorRepository = new FakePrestadorRepository();
        var gateway = new FakePagamentoGateway();

        var service = new PagamentoService(pagamentoRepository, agendamentoRepository, prestadorRepository, gateway);

        var resultado = await service.IniciarCheckoutAsync(agendamento.Id, agendamento.ClienteId);

        Assert.Equal(300, resultado.Valor);
        Assert.Equal(300, pagamento.Valor);
        Assert.Equal(300, gateway.UltimoCheckoutRequest!.Valor);
    }
}
