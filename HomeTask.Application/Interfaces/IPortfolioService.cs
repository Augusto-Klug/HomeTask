using HomeTask.Domain.Entities;

namespace HomeTask.Application.Interfaces;

public interface IPortfolioService
{
    Task<Portfolio> AdicionarAsync(Guid prestadorId, Stream imagem, string nomeArquivo, string? titulo, string? descricao, CancellationToken cancellationToken = default);
    Task<IEnumerable<Portfolio>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}