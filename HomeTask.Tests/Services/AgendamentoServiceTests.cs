using HomeTask.Application.Dtos;
using HomeTask.Application.Services;
using HomeTask.Domain.Enums;
using HomeTask.Tests.Helpers;

namespace HomeTask.Tests.Services;

public class AgendamentoServiceTests
{
    [Fact]
    public async Task CriarAsync_QuandoServicoPrestadorValido_DeveCriarSolicitacaoComValorDuracaoEEndereco()
    {
        var agendamentoRepository = new FakeAgendamentoRepository();
        var prestadorRepository = new FakePrestadorRepository();
        var service = new AgendamentoService(agendamentoRepository, prestadorRepository);
        var clienteId = Guid.NewGuid();
        var prestadorId = Guid.NewGuid();
        var endereco = EntidadeFactory.CriarEndereco(usuarioId: Guid.NewGuid());
        var servico = EntidadeFactory.CriarServicoPrestador(prestadorId: prestadorId, preco: 250);
        agendamentoRepository.EnderecoPrincipal = endereco;
        agendamentoRepository.Servicos.Add(servico);
        var dto = new AgendamentoDto
        {
            ClienteId = clienteId,
            DataHoraAgendada = DateTime.UtcNow.AddDays(1),
            Observacoes = "Periodo da manha",
            ServicosOferecidosIds = [servico.Id]
        };

        var resultado = await service.CriarAsync(dto);

        Assert.Equal(StatusAgendamento.Solicitado, resultado.Status);
        Assert.Equal(clienteId, resultado.ClienteId);
        Assert.Equal(prestadorId, resultado.PrestadorId);
        Assert.Equal(endereco.Id, resultado.EnderecoId);
        Assert.Equal(250, resultado.ValorTotal);
        Assert.Equal(120, resultado.DuracaoMinutos);
        Assert.Single(resultado.ServicosOferecidosIds);
        Assert.Equal(1, agendamentoRepository.AdicionarChamadas);
        Assert.Equal(1, agendamentoRepository.SalvarChamadas);
    }

    [Fact]
    public async Task CriarAsync_QuandoNaoEncontrarServico_DeveLancarErroENaoSalvar()
    {
        var agendamentoRepository = new FakeAgendamentoRepository();
        var service = new AgendamentoService(agendamentoRepository, new FakePrestadorRepository());
        var dto = new AgendamentoDto { ClienteId = Guid.NewGuid(), ServicosOferecidosIds = [Guid.NewGuid()] };

        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CriarAsync(dto));

        Assert.Equal("Nenhum servico valido encontrado para o agendamento.", excecao.Message);
        Assert.Equal(0, agendamentoRepository.AdicionarChamadas);
        Assert.Equal(0, agendamentoRepository.SalvarChamadas);
    }

    [Fact]
    public async Task AceitarAsync_QuandoSolicitado_DeveAlterarStatusParaAceito()
    {
        var agendamentoRepository = new FakeAgendamentoRepository();
        var agendamento = EntidadeFactory.CriarAgendamento(status: StatusAgendamento.Solicitado);
        agendamentoRepository.Seed(agendamento);
        var service = new AgendamentoService(agendamentoRepository, new FakePrestadorRepository());

        var resultado = await service.AceitarAsync(agendamento.Id);

        Assert.Equal(StatusAgendamento.Aceito, resultado.Status);
        Assert.NotNull(resultado.DataResposta);
        Assert.Equal(1, agendamentoRepository.AtualizarChamadas);
        Assert.Equal(1, agendamentoRepository.SalvarChamadas);
    }

    [Fact]
    public async Task IniciarAsync_QuandoNaoAceito_DeveLancarErro()
    {
        var agendamentoRepository = new FakeAgendamentoRepository();
        var agendamento = EntidadeFactory.CriarAgendamento(status: StatusAgendamento.Solicitado);
        agendamentoRepository.Seed(agendamento);
        var service = new AgendamentoService(agendamentoRepository, new FakePrestadorRepository());

        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => service.IniciarAsync(agendamento.Id));

        Assert.Equal("Agendamento nao pode ser iniciado neste status", excecao.Message);
        Assert.Equal(0, agendamentoRepository.AtualizarChamadas);
        Assert.Equal(0, agendamentoRepository.SalvarChamadas);
    }

    [Fact]
    public async Task ConcluirAsync_QuandoEmAndamento_DeveMoverParaAguardandoPagamentoSemIncrementarServicosDoPrestador()
    {
        var agendamentoRepository = new FakeAgendamentoRepository();
        var prestadorRepository = new FakePrestadorRepository();
        var prestador = EntidadeFactory.CriarPrestador();
        var agendamento = EntidadeFactory.CriarAgendamento(prestadorId: prestador.Id, status: StatusAgendamento.EmAndamento);
        agendamentoRepository.Seed(agendamento);
        prestadorRepository.Seed(prestador);
        var service = new AgendamentoService(agendamentoRepository, prestadorRepository);

        var resultado = await service.ConcluirAsync(agendamento.Id);

        Assert.Equal(StatusAgendamento.AguardandoPagamento, resultado.Status);
        Assert.NotNull(resultado.DataConclusao);
        Assert.Equal(0, prestador.TotalServicosConcluidos);
        Assert.Equal(0, prestadorRepository.AtualizarChamadas);
        Assert.Equal(1, agendamentoRepository.AtualizarChamadas);
    }
}
