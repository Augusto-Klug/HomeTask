using HomeTask.Application.Services;
using HomeTask.Tests.Helpers;

namespace HomeTask.Tests.Services;

public class PortfolioServiceTests
{
    [Fact]
    public async Task AdicionarAsync_DeveSalvarArquivoEPersistirPortfolio()
    {
        // Arrange
        var repository = new FakePortfolioRepository();
        var arquivoService = new FakeArquivoService { UrlRetornada = "/uploads/portfolio/foto.png" };
        var service = new PortfolioService(repository, arquivoService);
        var prestadorId = Guid.NewGuid();
        await using var imagem = new MemoryStream([1, 2, 3]);

        // Act
        var resultado = await service.AdicionarAsync(prestadorId, imagem, "foto.png", "Antes e depois", "Descricao");

        // Assert
        Assert.Equal(prestadorId, resultado.PrestadorId);
        Assert.Equal("/uploads/portfolio/foto.png", resultado.UrlImagem);
        Assert.Equal($"portfolio/{prestadorId}", arquivoService.UltimaSubpasta);
        Assert.Equal(1, arquivoService.SalvarChamadas);
        Assert.Equal(1, repository.AdicionarChamadas);
        Assert.Equal(1, repository.SalvarChamadas);
    }

    [Fact]
    public async Task RemoverAsync_QuandoPortfolioExiste_DeveExcluirArquivoERemoverRegistro()
    {
        // Arrange
        var repository = new FakePortfolioRepository();
        var arquivoService = new FakeArquivoService();
        var portfolio = new HomeTask.Domain.Entidades.Portfolio();
        portfolio.DefinirDados(Guid.NewGuid(), "/uploads/portfolio/foto.png", "Titulo", null, DateTime.UtcNow);
        repository.Seed(portfolio);
        var service = new PortfolioService(repository, arquivoService);

        // Act
        await service.RemoverAsync(portfolio.Id);

        // Assert
        Assert.Equal("/uploads/portfolio/foto.png", arquivoService.UltimaUrlExcluida);
        Assert.Equal(1, arquivoService.ExcluirChamadas);
        Assert.Equal(1, repository.RemoverChamadas);
        Assert.Equal(1, repository.SalvarChamadas);
    }

    [Fact]
    public async Task RemoverAsync_QuandoPortfolioNaoExiste_DeveNaoExcluirArquivoNemSalvar()
    {
        // Arrange
        var repository = new FakePortfolioRepository();
        var arquivoService = new FakeArquivoService();
        var service = new PortfolioService(repository, arquivoService);

        // Act
        await service.RemoverAsync(Guid.NewGuid());

        // Assert
        Assert.Equal(0, arquivoService.ExcluirChamadas);
        Assert.Equal(0, repository.RemoverChamadas);
        Assert.Equal(0, repository.SalvarChamadas);
    }
}
