using HomeTask.WebApi.Models.Entities;
using HomeTask.WebApi.Models.Enums;

namespace HomeTask.WebApi.Services.Interfaces;

/// <summary>
/// Interface para busca de serviços (RF03, RF07)
/// </summary>
public interface IServicoService
{
    Task<ServicoOferecido?> ObterPorIdAsync(int id);
    Task<ServicoOferecido> CriarAsync(ServicoOferecido servico);
    Task<ServicoOferecido> AtualizarAsync(ServicoOferecido servico);
    Task<bool> RemoverAsync(int id);
    Task<IEnumerable<ServicoOferecido>> ObterPorPrestadorAsync(int prestadorId);
    Task<IEnumerable<ServicoOferecido>> BuscarAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo);
}
