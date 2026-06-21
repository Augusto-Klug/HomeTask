using HomeTask.Application.Services;
using HomeTask.Domain.Entidades;
using HomeTask.Tests.Helpers;

namespace HomeTask.Tests.Services;

public class MensagemServiceTests
{
    [Fact]
    public async Task EnviarPorAgendamentoAsync_DevePersistirMensagemComAgendamentoERemetenteAutenticado()
    {
        var repository = new FakeMensagemRepository();
        var service = new MensagemService(repository);
        var agendamentoId = Guid.NewGuid();
        var remetenteId = Guid.NewGuid();

        var resultado = await service.EnviarPorAgendamentoAsync(agendamentoId, remetenteId, "  Olá!  ");

        Assert.Equal(agendamentoId, resultado.AgendamentoId);
        Assert.Equal(remetenteId, resultado.RemetenteId);
        Assert.Equal("Olá!", resultado.Conteudo);
        Assert.False(resultado.Lida);
        Assert.Equal(1, repository.AdicionarChamadas);
        Assert.Equal(1, repository.SalvarChamadas);
    }

    [Fact]
    public async Task EnviarPorAgendamentoAsync_QuandoTresUltimasSaoDoMesmoRemetente_DeveBloquear()
    {
        var repository = new FakeMensagemRepository();
        var service = new MensagemService(repository);
        var agendamentoId = Guid.NewGuid();
        var remetenteId = Guid.NewGuid();
        repository.Seed(
            CriarMensagem(agendamentoId, remetenteId, DateTime.UtcNow.AddMinutes(-3)),
            CriarMensagem(agendamentoId, remetenteId, DateTime.UtcNow.AddMinutes(-2)),
            CriarMensagem(agendamentoId, remetenteId, DateTime.UtcNow.AddMinutes(-1)));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.EnviarPorAgendamentoAsync(agendamentoId, remetenteId, "Mais uma mensagem"));

        Assert.Contains("Aguarde a resposta", ex.Message);
        Assert.Equal(0, repository.AdicionarChamadas);
    }

    [Fact]
    public async Task EnviarPorAgendamentoAsync_QuandoOutroParticipanteRespondeu_DevePermitirNovoEnvio()
    {
        var repository = new FakeMensagemRepository();
        var service = new MensagemService(repository);
        var agendamentoId = Guid.NewGuid();
        var remetenteId = Guid.NewGuid();
        var outroParticipanteId = Guid.NewGuid();
        repository.Seed(
            CriarMensagem(agendamentoId, remetenteId, DateTime.UtcNow.AddMinutes(-4)),
            CriarMensagem(agendamentoId, remetenteId, DateTime.UtcNow.AddMinutes(-3)),
            CriarMensagem(agendamentoId, outroParticipanteId, DateTime.UtcNow.AddMinutes(-2)));

        var resultado = await service.EnviarPorAgendamentoAsync(agendamentoId, remetenteId, "Pode seguir");

        Assert.Equal("Pode seguir", resultado.Conteudo);
        Assert.Equal(1, repository.AdicionarChamadas);
    }

    [Fact]
    public async Task RemoverExpiradasAsync_DeveExcluirMensagensComMaisDeTrintaDias()
    {
        var repository = new FakeMensagemRepository();
        var service = new MensagemService(repository);
        var agendamentoId = Guid.NewGuid();
        repository.Seed(
            CriarMensagem(agendamentoId, Guid.NewGuid(), DateTime.UtcNow.AddDays(-31)),
            CriarMensagem(agendamentoId, Guid.NewGuid(), DateTime.UtcNow.AddDays(-1)));

        var removidas = await service.RemoverExpiradasAsync();
        var restantes = (await repository.ObterPorAgendamentoAsync(agendamentoId, DateTime.MinValue)).ToList();

        Assert.Equal(1, removidas);
        Assert.Single(restantes);
    }

    private static Mensagem CriarMensagem(Guid agendamentoId, Guid remetenteId, DateTime dataEnvio)
    {
        var mensagem = new Mensagem();
        mensagem.DefinirDados(
            Guid.NewGuid(),
            remetenteId,
            null,
            agendamentoId,
            "Mensagem",
            dataEnvio,
            null,
            false);
        return mensagem;
    }
}
