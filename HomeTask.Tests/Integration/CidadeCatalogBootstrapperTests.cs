using HomeTask.Domain.Entidades;
using HomeTask.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeTask.Tests.Integration;

public class CidadeCatalogBootstrapperTests
{
    [Fact]
    public async Task SincronizarAsync_DevePopularTodasAsCidadesEPreservarIdsExistentes()
    {
        using var context = new SqliteTestContext();
        var cidadeExistente = new Cidade();
        var idBlumenau = Guid.Parse("20000000-0000-0000-0000-000000000001");
        cidadeExistente.DefinirDados(idBlumenau, "Blumenau", "SC", "4202404");

        context.Db.Cidades.Add(cidadeExistente);
        await context.Db.SaveChangesAsync();

        var bootstrapper = new CidadeCatalogBootstrapper(context.Db);

        await bootstrapper.SincronizarAsync();

        var total = await context.Db.Cidades.CountAsync();
        var blumenau = await context.Db.Cidades.SingleAsync(cidade => cidade.CodIBGE == "4202404");
        var saoPaulo = await context.Db.Cidades.SingleOrDefaultAsync(cidade => cidade.Nome == "São Paulo" && cidade.Estado == "SP");
        var rioBranco = await context.Db.Cidades.SingleOrDefaultAsync(cidade => cidade.Nome == "Rio Branco" && cidade.Estado == "AC");

        Assert.Equal(5571, total);
        Assert.Equal(idBlumenau, blumenau.Id);
        Assert.NotNull(saoPaulo);
        Assert.NotNull(rioBranco);
    }
}
