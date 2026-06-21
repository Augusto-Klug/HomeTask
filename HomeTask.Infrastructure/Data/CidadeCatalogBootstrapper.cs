using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using HomeTask.Domain.Entidades;
using Microsoft.EntityFrameworkCore;

namespace HomeTask.Infrastructure.Data;

public sealed class CidadeCatalogBootstrapper
{
    private const string ResourceName = "HomeTask.Infrastructure.Data.cidades-ibge.json";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HomeTaskDbContext _dbContext;

    public CidadeCatalogBootstrapper(HomeTaskDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SincronizarAsync(CancellationToken cancellationToken = default)
    {
        if (await _dbContext.Cidades.AnyAsync(cancellationToken))
        {
            var totalComCodIbge = await _dbContext.Cidades.CountAsync(
                cidade => cidade.CodIBGE != null && cidade.CodIBGE != string.Empty,
                cancellationToken);

            if (totalComCodIbge >= 5571)
                return;
        }

        var catalogo = await CarregarCatalogoAsync(cancellationToken);
        var cidadesExistentes = await _dbContext.Cidades
            .Where(cidade => cidade.CodIBGE != null)
            .ToDictionaryAsync(cidade => cidade.CodIBGE!, cancellationToken);

        foreach (var item in catalogo)
        {
            if (cidadesExistentes.TryGetValue(item.CodIbge, out var cidadeExistente))
            {
                cidadeExistente.DefinirDados(
                    cidadeExistente.Id,
                    item.Nome,
                    item.Estado,
                    item.CodIbge);
                continue;
            }

            var cidade = new Cidade();
            cidade.DefinirDados(
                GerarIdDeterministico(item.CodIbge),
                item.Nome,
                item.Estado,
                item.CodIbge);
            _dbContext.Cidades.Add(cidade);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task<IReadOnlyList<CidadeCatalogItem>> CarregarCatalogoAsync(CancellationToken cancellationToken)
    {
        await using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException($"Recurso '{ResourceName}' nao encontrado.");

        var catalogo = await JsonSerializer.DeserializeAsync<List<CidadeCatalogItem>>(stream, JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("Nao foi possivel desserializar o catalogo de cidades.");

        return catalogo;
    }

    private static Guid GerarIdDeterministico(string codIbge)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes($"cidade:{codIbge}"));
        return new Guid(hash);
    }

    private sealed class CidadeCatalogItem
    {
        public string CodIbge { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}
