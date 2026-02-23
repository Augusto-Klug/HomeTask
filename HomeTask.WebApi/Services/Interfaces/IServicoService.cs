using HomeTask.WebApi.Models.Entities;
using HomeTask.WebApi.Models.Enums;

namespace HomeTask.WebApi.Services.Interfaces;

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
