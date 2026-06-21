using HomeTask.Application.Dtos;
using HomeTask.Application.Services;
using HomeTask.Domain.Enums;
using HomeTask.Tests.Helpers;

namespace HomeTask.Tests.Services;

public class AvaliacaoServiceTests
{
    [Fact]
    public async Task CriarAsync_QuandoAgendamentoConcluidoComServicoPrestador_DevePersistirAvaliacaoDupla()
    {
        var avaliacaoRepository = new FakeAvaliacaoRepository();
        var prestadorRepository = new FakePrestadorRepository();
        var servicoRepository = new FakeServicoPrestadorRepository();
        var prestadorService = new PrestadorService(prestadorRepository);
        var servicoService = new ServicoPrestadorService(servicoRepository);
        var service = new AvaliacaoService(avaliacaoRepository, prestadorService, servicoService);

        var prestador = EntidadeFactory.CriarPrestador();
        var servico = EntidadeFactory.CriarServicoPrestador(prestadorId: prestador.Id);
        var agendamento = EntidadeFactory.CriarAgendamento(clienteId: Guid.NewGuid(), prestadorId: prestador.Id, status: StatusAgendamento.Concluido);
        var item = EntidadeFactory.CriarAgendamentoServico(agendamento.Id, servico.Id, servico.PrecoBase);
        EntidadeFactory.DefinirNavegacao(item, nameof(item.ServicoBase), servico);
        agendamento.AdicionarServico(item);

        avaliacaoRepository.AgendamentosElegiveis[agendamento.Id] = agendamento;
        prestadorRepository.Seed(prestador);
        servicoRepository.Seed(servico);

        var resultado = await service.CriarAsync(new AvaliacaoDto
        {
            AgendamentoId = agendamento.Id,
            ClienteId = agendamento.ClienteId,
            NotaServico = 5,
            NotaPrestador = 4,
            Comentario = "Muito bom"
        });

        Assert.Equal(servico.Id, resultado.ServicoPrestadorId);
        Assert.Equal(5, resultado.NotaServico);
        Assert.Equal(4, resultado.NotaPrestador);
        Assert.Equal(1, avaliacaoRepository.AdicionarChamadas);
        Assert.Equal(1, prestadorRepository.AtualizarChamadas);
        Assert.Equal(1, servicoRepository.AtualizarChamadas);
    }

    [Fact]
    public async Task CriarAsync_QuandoAgendamentoNaoConcluido_DeveFalhar()
    {
        var service = new AvaliacaoService(
            new FakeAvaliacaoRepository(),
            new PrestadorService(new FakePrestadorRepository()),
            new ServicoPrestadorService(new FakeServicoPrestadorRepository()));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CriarAsync(new AvaliacaoDto
        {
            AgendamentoId = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            NotaServico = 5,
            NotaPrestador = 5
        }));

        Assert.Equal("Somente servicos concluidos com pagamento aprovado podem ser avaliados", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_QuandoAgendamentoJaAvaliado_DeveImpedirSegundaAvaliacao()
    {
        var avaliacaoRepository = new FakeAvaliacaoRepository();
        var prestadorRepository = new FakePrestadorRepository();
        var servicoRepository = new FakeServicoPrestadorRepository();
        var service = new AvaliacaoService(
            avaliacaoRepository,
            new PrestadorService(prestadorRepository),
            new ServicoPrestadorService(servicoRepository));

        var prestador = EntidadeFactory.CriarPrestador();
        var servico = EntidadeFactory.CriarServicoPrestador(prestadorId: prestador.Id);
        var agendamento = EntidadeFactory.CriarAgendamento(clienteId: Guid.NewGuid(), prestadorId: prestador.Id, status: StatusAgendamento.Concluido);
        var item = EntidadeFactory.CriarAgendamentoServico(agendamento.Id, servico.Id, servico.PrecoBase);
        EntidadeFactory.DefinirNavegacao(item, nameof(item.ServicoBase), servico);
        agendamento.AdicionarServico(item);

        avaliacaoRepository.AgendamentosElegiveis[agendamento.Id] = agendamento;
        prestadorRepository.Seed(prestador);
        servicoRepository.Seed(servico);

        var existente = new HomeTask.Domain.Entidades.Avaliacao();
        existente.DefinirDados(Guid.NewGuid(), agendamento.Id, agendamento.ClienteId, prestador.Id, servico.Id, 3, 3, null, DateTime.UtcNow, true);
        avaliacaoRepository.Seed(existente);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CriarAsync(new AvaliacaoDto
        {
            AgendamentoId = agendamento.Id,
            ClienteId = agendamento.ClienteId,
            NotaServico = 5,
            NotaPrestador = 5
        }));

        Assert.Equal("Este servico ja foi avaliado", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_QuandoAgendamentoTemServicoPrincipalPersistido_DeveUsarServicoPrincipal()
    {
        var avaliacaoRepository = new FakeAvaliacaoRepository();
        var prestadorRepository = new FakePrestadorRepository();
        var servicoRepository = new FakeServicoPrestadorRepository();
        var service = new AvaliacaoService(
            avaliacaoRepository,
            new PrestadorService(prestadorRepository),
            new ServicoPrestadorService(servicoRepository));

        var prestador = EntidadeFactory.CriarPrestador();
        var servicoPrestador = EntidadeFactory.CriarServicoPrestador(prestadorId: prestador.Id);
        var servicoCliente = EntidadeFactory.CriarServicoCliente();
        var agendamento = EntidadeFactory.CriarAgendamento(clienteId: Guid.NewGuid(), prestadorId: prestador.Id, principalServicoPrestadorId: servicoPrestador.Id, status: StatusAgendamento.Concluido);
        var item = EntidadeFactory.CriarAgendamentoServico(agendamento.Id, servicoCliente.Id, servicoCliente.PrecoBase);
        EntidadeFactory.DefinirNavegacao(item, nameof(item.ServicoBase), servicoCliente);
        agendamento.AdicionarServico(item);

        avaliacaoRepository.AgendamentosElegiveis[agendamento.Id] = agendamento;
        prestadorRepository.Seed(prestador);
        servicoRepository.Seed(servicoPrestador);

        var resultado = await service.CriarAsync(new AvaliacaoDto
        {
            AgendamentoId = agendamento.Id,
            ClienteId = agendamento.ClienteId,
            NotaServico = 5,
            NotaPrestador = 4
        });

        Assert.Equal(servicoPrestador.Id, resultado.ServicoPrestadorId);
    }

    [Fact]
    public async Task CriarAsync_QuandoAgendamentoTemPedidoDoClienteSemServicoPrincipal_DeveResolverPorCategoriaDoPrestador()
    {
        var avaliacaoRepository = new FakeAvaliacaoRepository();
        var prestadorRepository = new FakePrestadorRepository();
        var servicoRepository = new FakeServicoPrestadorRepository();
        var service = new AvaliacaoService(
            avaliacaoRepository,
            new PrestadorService(prestadorRepository),
            new ServicoPrestadorService(servicoRepository));

        var prestador = EntidadeFactory.CriarPrestador();
        var servicoPrestador = EntidadeFactory.CriarServicoPrestador(prestadorId: prestador.Id, categoria: CategoriaServico.Reparos);
        var servicoCliente = EntidadeFactory.CriarServicoCliente(categoria: CategoriaServico.Reparos);
        var agendamento = EntidadeFactory.CriarAgendamento(clienteId: Guid.NewGuid(), prestadorId: prestador.Id, status: StatusAgendamento.Concluido);
        var item = EntidadeFactory.CriarAgendamentoServico(agendamento.Id, servicoCliente.Id, servicoCliente.PrecoBase);
        EntidadeFactory.DefinirNavegacao(item, nameof(item.ServicoBase), servicoCliente);
        agendamento.AdicionarServico(item);

        avaliacaoRepository.AgendamentosElegiveis[agendamento.Id] = agendamento;
        prestadorRepository.Seed(prestador);
        servicoRepository.Seed(servicoPrestador);

        var resultado = await service.CriarAsync(new AvaliacaoDto
        {
            AgendamentoId = agendamento.Id,
            ClienteId = agendamento.ClienteId,
            NotaServico = 5,
            NotaPrestador = 5
        });

        Assert.Equal(servicoPrestador.Id, resultado.ServicoPrestadorId);
    }
}
