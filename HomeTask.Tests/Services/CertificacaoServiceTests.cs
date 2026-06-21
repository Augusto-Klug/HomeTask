using HomeTask.Application.Services;
using HomeTask.Tests.Helpers;

namespace HomeTask.Tests.Services;

public class CertificacaoServiceTests
{
    [Fact]
    public async Task AdicionarAsync_QuandoDocumentoInformado_DeveSalvarArquivoEPersistirCertificacao()
    {
        // Arrange
        var repository = new FakeCertificacaoRepository();
        var arquivoService = new FakeArquivoService { UrlRetornada = "/uploads/certificacoes/doc.pdf" };
        var service = new CertificacaoService(repository, arquivoService);
        var prestadorId = Guid.NewGuid();
        await using var documento = new MemoryStream([1, 2, 3]);

        // Act
        var resultado = await service.AdicionarAsync(prestadorId, documento, "certificado.pdf", "NR10", "Instituto", DateTime.UtcNow.Date, null);

        // Assert
        Assert.Equal(prestadorId, resultado.PrestadorId);
        Assert.Equal("NR10", resultado.Nome);
        Assert.Equal("/uploads/certificacoes/doc.pdf", resultado.UrlDocumento);
        Assert.Equal($"certificacoes/{prestadorId}", arquivoService.UltimaSubpasta);
        Assert.Equal(1, arquivoService.SalvarChamadas);
        Assert.Equal(1, repository.AdicionarChamadas);
        Assert.Equal(1, repository.SalvarChamadas);
    }

    [Fact]
    public async Task AdicionarAsync_QuandoDocumentoNaoInformado_DevePersistirSemSalvarArquivo()
    {
        // Arrange
        var repository = new FakeCertificacaoRepository();
        var arquivoService = new FakeArquivoService();
        var service = new CertificacaoService(repository, arquivoService);

        // Act
        var resultado = await service.AdicionarAsync(Guid.NewGuid(), null, null, "Curso", null, null, null);

        // Assert
        Assert.Null(resultado.UrlDocumento);
        Assert.Equal(0, arquivoService.SalvarChamadas);
        Assert.Equal(1, repository.AdicionarChamadas);
        Assert.Equal(1, repository.SalvarChamadas);
    }

    [Fact]
    public async Task RemoverAsync_QuandoCertificacaoTemDocumento_DeveExcluirArquivoERemoverRegistro()
    {
        // Arrange
        var repository = new FakeCertificacaoRepository();
        var arquivoService = new FakeArquivoService();
        var certificacao = new HomeTask.Domain.Entidades.Certificacao();
        certificacao.DefinirDados(Guid.NewGuid(), "Curso", null, null, null, "/uploads/certificacoes/doc.pdf", DateTime.UtcNow);
        repository.Seed(certificacao);
        var service = new CertificacaoService(repository, arquivoService);

        // Act
        await service.RemoverAsync(certificacao.Id);

        // Assert
        Assert.Equal("/uploads/certificacoes/doc.pdf", arquivoService.UltimaUrlExcluida);
        Assert.Equal(1, arquivoService.ExcluirChamadas);
        Assert.Equal(1, repository.RemoverChamadas);
        Assert.Equal(1, repository.SalvarChamadas);
    }
}
