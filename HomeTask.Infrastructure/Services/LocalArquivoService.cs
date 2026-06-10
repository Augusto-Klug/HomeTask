using HomeTask.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace HomeTask.Infrastructure.Services;

public class LocalArquivoService : IArquivoService
{
    private readonly string _raiz;
    private readonly string _urlBase;

    public LocalArquivoService(IWebHostEnvironment env, IConfiguration config)
    {
        _raiz = Path.Combine(env.ContentRootPath, "uploads");
        _urlBase = config["Arquivos:UrlBase"] ?? "/uploads";
    }

    public async Task<string> SalvarAsync(Stream conteudo, string nomeArquivo, string subpasta, CancellationToken cancellationToken = default)
    {
        var pasta = Path.Combine(_raiz, subpasta);
        Directory.CreateDirectory(pasta);

        // Nome único para evitar colisões
        var extensao = Path.GetExtension(nomeArquivo);
        var nomeUnico = $"{Guid.NewGuid()}{extensao}";
        var caminho = Path.Combine(pasta, nomeUnico);

        await using var arquivo = File.Create(caminho);
        await conteudo.CopyToAsync(arquivo, cancellationToken);

        return $"{_urlBase}/{subpasta}/{nomeUnico}";
    }

    public Task ExcluirAsync(string url, CancellationToken cancellationToken = default)
    {
        var caminho = Path.Combine(_raiz, url.Replace(_urlBase + "/", "").Replace("/", Path.DirectorySeparatorChar.ToString()));
        if (File.Exists(caminho))
            File.Delete(caminho);

        return Task.CompletedTask;
    }
}
