using HomeTask.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace HomeTask.Tests.Infrastructure;

public class LocalArquivoServiceTests : IDisposable
{
    private readonly string _contentRoot = Path.Combine(Path.GetTempPath(), "HomeTaskTests", Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task SalvarAsync_DeveCriarArquivoComNomeUnicoERetornarUrlPublica()
    {
        // Arrange
        var service = CriarService();
        await using var conteudo = new MemoryStream([1, 2, 3, 4]);

        // Act
        var url = await service.SalvarAsync(conteudo, "foto.jpg", "portfolio/prestador");
        var caminho = ObterCaminhoFisico(url);

        // Assert
        Assert.StartsWith("/arquivos/portfolio/prestador/", url);
        Assert.EndsWith(".jpg", url);
        Assert.True(File.Exists(caminho));
        Assert.Equal([1, 2, 3, 4], File.ReadAllBytes(caminho));
    }

    [Fact]
    public async Task ExcluirAsync_QuandoArquivoExiste_DeveRemoverArquivoFisico()
    {
        // Arrange
        var service = CriarService();
        await using var conteudo = new MemoryStream([5, 6, 7]);
        var url = await service.SalvarAsync(conteudo, "documento.pdf", "certificacoes/prestador");
        var caminho = ObterCaminhoFisico(url);

        // Act
        await service.ExcluirAsync(url);

        // Assert
        Assert.False(File.Exists(caminho));
    }

    public void Dispose()
    {
        if (Directory.Exists(_contentRoot))
            Directory.Delete(_contentRoot, recursive: true);
    }

    private LocalArquivoService CriarService()
    {
        Directory.CreateDirectory(_contentRoot);
        var env = new FakeWebHostEnvironment(_contentRoot);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Arquivos:UrlBase"] = "/arquivos" })
            .Build();

        return new LocalArquivoService(env, config);
    }

    private string ObterCaminhoFisico(string url) =>
        Path.Combine(_contentRoot, "uploads", url.Replace("/arquivos/", string.Empty).Replace('/', Path.DirectorySeparatorChar));

    private sealed class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public FakeWebHostEnvironment(string contentRootPath)
        {
            ContentRootPath = contentRootPath;
            WebRootPath = contentRootPath;
            ContentRootFileProvider = new NullFileProvider();
            WebRootFileProvider = new NullFileProvider();
        }

        public string EnvironmentName { get; set; } = "Test";
        public string ApplicationName { get; set; } = "HomeTask.Tests";
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}
