using HomeTask.Application.Services;
using HomeTask.Tests.Helpers;

namespace HomeTask.Tests.Services;

public class ConversaServiceTests
{
    [Fact]
    public async Task ObterOuCriarAsync_QuandoConversaExiste_DeveRetornarExistenteSemSalvar()
    {
        // Arrange
        var repository = new FakeConversaRepository();
        var clienteId = Guid.NewGuid();
        var prestadorId = Guid.NewGuid();
        var conversa = new HomeTask.Domain.Entidades.Conversa();
        conversa.DefinirDados(clienteId, prestadorId, DateTime.UtcNow.AddDays(-1));
        repository.Seed(conversa);
        var service = new ConversaService(repository);

        // Act
        var resultado = await service.ObterOuCriarAsync(clienteId, prestadorId);

        // Assert
        Assert.Equal(conversa.Id, resultado.Id);
        Assert.Equal(0, repository.AdicionarChamadas);
        Assert.Equal(0, repository.SalvarChamadas);
    }

    [Fact]
    public async Task ObterOuCriarAsync_QuandoNaoExiste_DeveCriarConversa()
    {
        // Arrange
        var repository = new FakeConversaRepository();
        var clienteId = Guid.NewGuid();
        var prestadorId = Guid.NewGuid();
        var service = new ConversaService(repository);

        // Act
        var resultado = await service.ObterOuCriarAsync(clienteId, prestadorId);

        // Assert
        Assert.Equal(clienteId, resultado.ClienteId);
        Assert.Equal(prestadorId, resultado.PrestadorId);
        Assert.Equal(1, repository.AdicionarChamadas);
        Assert.Equal(1, repository.SalvarChamadas);
    }
}
