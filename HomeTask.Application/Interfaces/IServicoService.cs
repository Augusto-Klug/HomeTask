using HomeTask.Domain.Entities;
using HomeTask.Domain.Enums;

namespace HomeTask.Application.Interfaces;

/// <summary>
/// Interface para busca de serviços (RF03, RF07)
/// </summary>
public interface IServicoService
{
    Task<ServicoOferecido?> ObterPorIdAsync(Guid id);
    Task<ServicoOferecido> CriarAsync(ServicoOferecido servico);
    Task<ServicoOferecido> AtualizarAsync(ServicoOferecido servico);
    Task<bool> RemoverAsync(Guid id);
    Task<IEnumerable<ServicoOferecido>> ObterPorPrestadorAsync(Guid prestadorId);
    Task<IEnumerable<ServicoOferecido>> BuscarAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo);
}
