using HomeTask.Domain.Entidades;

namespace HomeTask.Application.Interfaces;

public interface ICertificacaoService
{
    Task<Certificacao> AdicionarAsync(Guid prestadorId, Stream documento, string nomeArquivo, string nome, string? instituicao, DateTime? dataEmissao, DateTime? dataValidade, CancellationToken cancellationToken = default);
    Task<IEnumerable<Certificacao>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}