using HomeTask.Application.Services;
using HomeTask.Tests.Helpers;

namespace HomeTask.Tests.Services;

public class CidadeServiceTests
{
    [Fact]
    public async Task ListarAsync_DeveMapearCidadesOrdenadasDoRepositorio()
    {
        // Arrange
        var repository = new FakeCidadeRepository();
        repository.Seed(
            EntidadeFactory.CriarCidade(nome: "Curitiba", estado: "PR"),
            EntidadeFactory.CriarCidade(nome: "Blumenau", estado: "SC"));
        var service = new CidadeService(repository);

        // Act
        var resultado = (await service.ListarAsync()).ToList();

        // Assert
        Assert.Equal(2, resultado.Count);
        Assert.Equal("Curitiba", resultado[0].Nome);
        Assert.Equal("Blumenau", resultado[1].Nome);
    }

    [Fact]
    public async Task BuscarAsync_QuandoTermoInformado_DeveRetornarSomenteCidadesEncontradas()
    {
        // Arrange
        var repository = new FakeCidadeRepository();
        repository.Seed(
            EntidadeFactory.CriarCidade(nome: "Blumenau", estado: "SC"),
            EntidadeFactory.CriarCidade(nome: "Joinville", estado: "SC"),
            EntidadeFactory.CriarCidade(nome: "Curitiba", estado: "PR"));
        var service = new CidadeService(repository);

        // Act
        var resultado = (await service.BuscarAsync("ville")).ToList();

        // Assert
        Assert.Single(resultado);
        Assert.Equal("Joinville", resultado[0].Nome);
    }
}
