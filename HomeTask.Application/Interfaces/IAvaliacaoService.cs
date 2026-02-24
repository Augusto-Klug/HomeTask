using HomeTask.Domain.Entities;

namespace HomeTask.Application.Interfaces;

/// <summary>
/// Interface para gerenciamento de avaliações (RF06, NEG06, NEG07)
/// </summary>
public interface IAvaliacaoService
{
    Task<Avaliacao?> ObterPorIdAsync(Guid id);
    Task<Avaliacao?> ObterPorAgendamentoAsync(Guid agendamentoId);
    Task<Avaliacao> CriarAsync(Avaliacao avaliacao);
    Task<IEnumerable<Avaliacao>> ObterPorPrestadorAsync(Guid prestadorId);
    Task<IEnumerable<Avaliacao>> ObterPorClienteAsync(Guid clienteId);
    Task<bool> PodeAvaliarAsync(Guid clienteId, Guid agendamentoId);
}
