using HomeTask.WebApi.Models.Entities;
using HomeTask.WebApi.Models.Enums;

namespace HomeTask.WebApi.Services.Interfaces;

/// <summary>
/// Interface para gerenciamento de prestadores (RF02, RF07, RF09, RF12)
/// </summary>
public interface IPrestadorService
{
    Task<Prestador?> ObterPorIdAsync(Guid id);
    Task<Prestador?> ObterPorUsuarioIdAsync(Guid usuarioId);
    Task<Prestador> CriarAsync(Prestador prestador);
    Task<Prestador> AtualizarAsync(Prestador prestador);
    Task<IEnumerable<Prestador>> BuscarAsync(CategoriaServico? categoria, string? cidade, DateTime? dataDisponivel);
    Task<IEnumerable<Agendamento>> ObterHistoricoServicosAsync(Guid prestadorId);
    Task AtualizarMediaAvaliacoesAsync(Guid prestadorId);
    Task AtualizarStatusAsync(Guid prestadorId, StatusPrestador status);
}
