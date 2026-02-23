using HomeTask.WebApi.Models.Entities;

namespace HomeTask.WebApi.Services.Interfaces;

/// <summary>
/// Interface para gerenciamento de avaliações (RF06, NEG06, NEG07)
/// </summary>
public interface IAvaliacaoService
{
    Task<Avaliacao?> ObterPorIdAsync(int id);
    Task<Avaliacao?> ObterPorAgendamentoAsync(int agendamentoId);
    Task<Avaliacao> CriarAsync(Avaliacao avaliacao);
    Task<IEnumerable<Avaliacao>> ObterPorPrestadorAsync(int prestadorId);
    Task<IEnumerable<Avaliacao>> ObterPorClienteAsync(int clienteId);
    Task<bool> PodeAvaliarAsync(int clienteId, int agendamentoId);
}
