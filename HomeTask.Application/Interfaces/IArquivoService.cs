namespace HomeTask.Application.Interfaces;

public interface IArquivoService
{
    Task<string> SalvarAsync(Stream conteudo, string nomeArquivo, string subpasta, CancellationToken cancellationToken = default);
    Task ExcluirAsync(string url, CancellationToken cancellationToken = default);
}