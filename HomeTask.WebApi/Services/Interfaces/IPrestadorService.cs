using HomeTask.WebApi.Models.Entities;
using HomeTask.WebApi.Models.Enums;

namespace HomeTask.WebApi.Services.Interfaces;

/// <summary>
/// Interface para gerenciamento de prestadores (RF02, RF07, RF09, RF12)
/// </summary>
public interface IPrestadorService
{
    Task<Prestador?> ObterPorIdAsync(int id);
    Task<Prestador?> ObterPorUsuarioIdAsync(int usuarioId);
    Task<Prestador> CriarAsync(Prestador prestador);
    Task<Prestador> AtualizarAsync(Prestador prestador);
    Task<IEnumerable<Prestador>> BuscarAsync(CategoriaServico? categoria, string? cidade, DateTime? dataDisponivel);
    Task<IEnumerable<Agendamento>> ObterHistoricoServicosAsync(int prestadorId);
    Task AtualizarMediaAvaliacoesAsync(int prestadorId);
    Task AtualizarStatusAsync(int prestadorId, StatusPrestador status);
}
