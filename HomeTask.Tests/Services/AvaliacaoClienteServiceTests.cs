using HomeTask.Application.Dtos;
using HomeTask.Application.Services;
using HomeTask.Domain.Enums;
using HomeTask.Tests.Helpers;

namespace HomeTask.Tests.Services;

public class AvaliacaoClienteServiceTests
{
    [Fact]
    public async Task CriarAsync_QuandoAgendamentoElegivel_DevePersistirAvaliacaoDoCliente()
    {
        var repository = new FakeAvaliacaoClienteRepository();
        var clienteRepository = new FakeClienteRepository();
        var service = new AvaliacaoClienteService(repository, new ClienteService(clienteRepository));

        var cliente = EntidadeFactory.CriarCliente();
        var agendamento = EntidadeFactory.CriarAgendamento(clienteId: cliente.Id, prestadorId: Guid.NewGuid(), status: StatusAgendamento.Concluido);
        repository.AgendamentosElegiveis[agendamento.Id] = agendamento;
        clienteRepository.Seed(cliente);

        var resultado = await service.CriarAsync(new AvaliacaoClienteDto
        {
            AgendamentoId = agendamento.Id,
            PrestadorId = agendamento.PrestadorId,
            ClienteId = Guid.Empty,
            Nota = 5,
            Comentario = "Cliente cumpriu o combinado"
        });

        Assert.Equal(cliente.Id, resultado.ClienteId);
        Assert.Equal(5, resultado.Nota);
        Assert.Equal(1, repository.AdicionarChamadas);
        Assert.Equal(1, clienteRepository.AtualizarChamadas);
    }

    [Fact]
    public async Task CriarAsync_QuandoAgendamentoNaoElegivel_DeveFalhar()
    {
        var service = new AvaliacaoClienteService(
            new FakeAvaliacaoClienteRepository(),
            new ClienteService(new FakeClienteRepository()));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CriarAsync(new AvaliacaoClienteDto
        {
            AgendamentoId = Guid.NewGuid(),
            PrestadorId = Guid.NewGuid(),
            Nota = 4
        }));

        Assert.Equal("Somente servicos concluidos com pagamento aprovado podem ser avaliados", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_QuandoJaExisteAvaliacaoNoAgendamento_DeveImpedirSegundaAvaliacao()
    {
        var repository = new FakeAvaliacaoClienteRepository();
        var clienteRepository = new FakeClienteRepository();
        var service = new AvaliacaoClienteService(repository, new ClienteService(clienteRepository));

        var cliente = EntidadeFactory.CriarCliente();
        var agendamento = EntidadeFactory.CriarAgendamento(clienteId: cliente.Id, prestadorId: Guid.NewGuid(), status: StatusAgendamento.Concluido);
        repository.AgendamentosElegiveis[agendamento.Id] = agendamento;
        clienteRepository.Seed(cliente);

        var existente = new HomeTask.Domain.Entidades.AvaliacaoCliente();
        existente.DefinirDados(Guid.NewGuid(), agendamento.Id, cliente.Id, agendamento.PrestadorId, 3, null, DateTime.UtcNow, true);
        repository.Seed(existente);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CriarAsync(new AvaliacaoClienteDto
        {
            AgendamentoId = agendamento.Id,
            PrestadorId = agendamento.PrestadorId,
            Nota = 5
        }));

        Assert.Equal("Este cliente ja foi avaliado neste agendamento", ex.Message);
    }
}
