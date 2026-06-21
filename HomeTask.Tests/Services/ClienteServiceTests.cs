using HomeTask.Application.Services;
using HomeTask.Tests.Helpers;

namespace HomeTask.Tests.Services;

public class ClienteServiceTests
{
    [Fact]
    public async Task AtualizarMediaAvaliacoesAsync_DeveRecalcularComBaseNasAvaliacoesRecebidas()
    {
        var repository = new FakeClienteRepository();
        var service = new ClienteService(repository);
        var cliente = EntidadeFactory.CriarCliente();
        var avaliacaoA = new HomeTask.Domain.Entidades.AvaliacaoCliente();
        avaliacaoA.DefinirDados(Guid.NewGuid(), Guid.NewGuid(), cliente.Id, Guid.NewGuid(), 4, null, DateTime.UtcNow, true);
        var avaliacaoB = new HomeTask.Domain.Entidades.AvaliacaoCliente();
        avaliacaoB.DefinirDados(Guid.NewGuid(), Guid.NewGuid(), cliente.Id, Guid.NewGuid(), 2, null, DateTime.UtcNow, true);
        EntidadeFactory.DefinirNavegacao(cliente, nameof(cliente.AvaliacoesRecebidas), new List<HomeTask.Domain.Entidades.AvaliacaoCliente> { avaliacaoA, avaliacaoB });
        repository.Seed(cliente);

        await service.AtualizarMediaAvaliacoesAsync(cliente.Id);

        Assert.Equal(3, cliente.MediaAvaliacoes);
        Assert.Equal(2, cliente.TotalAvaliacoes);
    }

    [Fact]
    public async Task ObterPerfilPublicoAsync_DeveRetornarSomenteMetricasResumo()
    {
        var repository = new FakeClienteRepository();
        var service = new ClienteService(repository);
        var usuario = EntidadeFactory.CriarUsuario(nome: "Pedro Martins");
        var cidade = EntidadeFactory.CriarCidade(nome: "Blumenau", estado: "SC");
        var endereco = EntidadeFactory.CriarEndereco(usuarioId: usuario.Id, cidadeId: cidade.Id);
        EntidadeFactory.DefinirNavegacao(endereco, nameof(endereco.Cidade), cidade);
        EntidadeFactory.DefinirNavegacao(usuario, nameof(usuario.Endereco), endereco);
        var cliente = EntidadeFactory.CriarCliente(usuarioId: usuario.Id);
        cliente.AtualizarMetricasAvaliacao(4.5m, 8);
        EntidadeFactory.DefinirNavegacao(cliente, nameof(cliente.Usuario), usuario);
        repository.Seed(cliente);
        repository.TotalServicosConcluidosPorCliente[cliente.Id] = 14;

        var perfil = await service.ObterPerfilPublicoAsync(cliente.Id);

        Assert.NotNull(perfil);
        Assert.Equal(cliente.Id, perfil!.Id);
        Assert.Equal("Pedro Martins", perfil.Nome);
        Assert.Equal("Blumenau", perfil.Cidade);
        Assert.Equal("SC", perfil.Estado);
        Assert.Equal(4.5m, perfil.MediaAvaliacoes);
        Assert.Equal(8, perfil.TotalAvaliacoes);
        Assert.Equal(14, perfil.TotalServicosContratados);
    }
}
