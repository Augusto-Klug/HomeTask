using HomeTask.Application.Dtos;

namespace HomeTask.Application.Interfaces;

public interface IPortfolioService
{
    Task<PortfolioDto> AdicionarAsync(Guid prestadorId, Stream imagem, string nomeArquivo, string? titulo, string? descricao, CancellationToken cancellationToken = default);
    Task<IEnumerable<PortfolioDto>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}
