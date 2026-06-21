using HomeTask.Application.Dtos;
using HomeTask.Application.Services;
using HomeTask.Domain.Entidades;
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
        var service = new AgendamentoService(agendamentoRepository, prestadorRepository, new FakeServicoPrestadorRepository());
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
            EnderecoDescricao = "Rua do cliente, 45 - Fundos",
            Observacoes = "Periodo da manha",
            ServicosOferecidosIds = [servico.Id]
        };

        var resultado = await service.CriarAsync(dto);

        Assert.Equal(StatusAgendamento.Solicitado, resultado.Status);
        Assert.Equal(clienteId, resultado.ClienteId);
        Assert.Equal(prestadorId, resultado.PrestadorId);
        Assert.Equal(endereco.Id, resultado.EnderecoId);
        Assert.Equal("Rua do cliente, 45 - Fundos", resultado.EnderecoDescricao);
        Assert.Equal(250, resultado.ValorTotal);
        Assert.Equal(120, resultado.DuracaoMinutos);
        Assert.Single(resultado.ServicosOferecidosIds);
        Assert.Equal(1, agendamentoRepository.AdicionarChamadas);
        Assert.Equal(1, agendamentoRepository.SalvarChamadas);
    }

    [Fact]
    public async Task CriarAsync_QuandoPedidoClienteForACombinar_DeveUsarValorPropostoEResolverServicoPrincipalAutomaticamente()
    {
        var agendamentoRepository = new FakeAgendamentoRepository();
        var servicoPrestadorRepository = new FakeServicoPrestadorRepository();
        var service = new AgendamentoService(agendamentoRepository, new FakePrestadorRepository(), servicoPrestadorRepository);
        var clienteId = Guid.NewGuid();
        var prestadorId = Guid.NewGuid();
        var endereco = EntidadeFactory.CriarEndereco(usuarioId: Guid.NewGuid());
        var pedidoCliente = EntidadeFactory.CriarServicoCliente(clienteId: clienteId, preco: 0);
        pedidoCliente.DefinirDados(
            pedidoCliente.Id,
            pedidoCliente.ClienteId,
            pedidoCliente.Categoria,
            pedidoCliente.Titulo,
            pedidoCliente.Descricao,
            pedidoCliente.PrecoBase,
            FormatoCobranca.ACombinar,
            pedidoCliente.DataDesejada,
            pedidoCliente.Ativo,
            pedidoCliente.DataCriacao);
        var servicoPrestador = EntidadeFactory.CriarServicoPrestador(prestadorId: prestadorId, preco: 180);

        agendamentoRepository.EnderecoPrincipal = endereco;
        agendamentoRepository.Servicos.Add(pedidoCliente);
        servicoPrestadorRepository.Seed(servicoPrestador);

        var dto = new AgendamentoDto
        {
            ClienteId = clienteId,
            PrestadorId = prestadorId,
            DataHoraAgendada = DateTime.UtcNow.AddDays(1),
            ServicosOferecidosIds = [pedidoCliente.Id],
            ValorProposto = 180
        };

        var resultado = await service.CriarAsync(dto);

        Assert.Equal(180, resultado.ValorTotal);
        Assert.Equal(servicoPrestador.Id, resultado.PrincipalServicoPrestadorId);
        Assert.Equal(120, resultado.DuracaoMinutos);
        Assert.Single(((Agendamento)agendamentoRepository.UltimoAdicionado!).AgendamentoServicos);
        Assert.Equal(180, ((Agendamento)agendamentoRepository.UltimoAdicionado!).AgendamentoServicos.Single().ValorUnitario);
    }

    [Fact]
    public async Task CriarAsync_QuandoNaoEncontrarServico_DeveLancarErroENaoSalvar()
    {
        var agendamentoRepository = new FakeAgendamentoRepository();
        var service = new AgendamentoService(agendamentoRepository, new FakePrestadorRepository(), new FakeServicoPrestadorRepository());
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
        var service = new AgendamentoService(agendamentoRepository, new FakePrestadorRepository(), new FakeServicoPrestadorRepository());

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
        var service = new AgendamentoService(agendamentoRepository, new FakePrestadorRepository(), new FakeServicoPrestadorRepository());

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
        var service = new AgendamentoService(agendamentoRepository, prestadorRepository, new FakeServicoPrestadorRepository());

        var resultado = await service.ConcluirAsync(agendamento.Id);

        Assert.Equal(StatusAgendamento.AguardandoPagamento, resultado.Status);
        Assert.NotNull(resultado.DataConclusao);
        Assert.Equal(0, prestador.TotalServicosConcluidos);
        Assert.Equal(0, prestadorRepository.AtualizarChamadas);
        Assert.Equal(1, agendamentoRepository.AtualizarChamadas);
    }

    [Fact]
    public async Task ConcluirAsync_QuandoServicoEhPorHora_DeveArredondarParaCimaEAtualizarValorFinal()
    {
        var agendamentoRepository = new FakeAgendamentoRepository();
        var prestadorRepository = new FakePrestadorRepository();
        var service = new AgendamentoService(agendamentoRepository, prestadorRepository, new FakeServicoPrestadorRepository());

        var prestador = EntidadeFactory.CriarPrestador();
        var servico = new ServicoPrestador();
        servico.DefinirDados(Guid.NewGuid(), prestador.Id, CategoriaServico.Faxina, "Limpeza", "Limpeza por hora", 100, FormatoCobranca.PorHora, 60, true, 0, 0, true, DateTime.UtcNow);

        var agendamento = EntidadeFactory.CriarAgendamento(prestadorId: prestador.Id, status: StatusAgendamento.EmAndamento);
        var inicio = DateTime.UtcNow.AddHours(-2).AddMinutes(-1);
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
            agendamento.ValorTotal,
            agendamento.DataSolicitacao,
            agendamento.DataResposta,
            inicio,
            agendamento.DataConclusao,
            agendamento.MotivoRecusa);

        var item = EntidadeFactory.CriarAgendamentoServico(agendamento.Id, servico.Id, servico.PrecoBase);
        EntidadeFactory.DefinirNavegacao(item, nameof(item.ServicoBase), servico);
        agendamento.AdicionarServico(item);

        agendamentoRepository.Seed(agendamento);
        prestadorRepository.Seed(prestador);

        var resultado = await service.ConcluirAsync(agendamento.Id);

        Assert.Equal(StatusAgendamento.AguardandoPagamento, resultado.Status);
        Assert.Equal(300, resultado.ValorTotal);
        Assert.Equal(3, agendamento.AgendamentoServicos.Single().Quantidade);
        Assert.Equal(100, agendamento.AgendamentoServicos.Single().ValorUnitario);
    }
}
