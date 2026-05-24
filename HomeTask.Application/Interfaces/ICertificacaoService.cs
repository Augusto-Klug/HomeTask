using HomeTask.Application.Dtos;

namespace HomeTask.Application.Interfaces;

public interface ICertificacaoService
{
    Task<CertificacaoDto> AdicionarAsync(Guid prestadorId, Stream documento, string nomeArquivo, string nome, string? instituicao, DateTime? dataEmissao, DateTime? dataValidade, CancellationToken cancellationToken = default);
    Task<IEnumerable<CertificacaoDto>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}
