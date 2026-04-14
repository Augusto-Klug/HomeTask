using HomeTask.Domain.Entities;
using HomeTask.Domain.Enums;

namespace HomeTask.Application.Interfaces;

/// <summary>
/// Interface para busca de serviços (RF03, RF07)
/// </summary>
public interface IServicoService
{
    Task<ServicoOferecido?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServicoOferecido> CriarAsync(ServicoOferecido servico, CancellationToken cancellationToken = default);
    Task<ServicoOferecido> AtualizarAsync(ServicoOferecido servico, CancellationToken cancellationToken = default);
    Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServicoOferecido>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    //Task<IEnumerable<ServicoOferecido>> BuscarAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServicoOferecido>> BuscarAsync(Guid? categoriaId, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken = default);
}
