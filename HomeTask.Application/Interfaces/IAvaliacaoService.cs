using HomeTask.Domain.Entities;

namespace HomeTask.Application.Interfaces;

/// <summary>
/// Interface para gerenciamento de avaliações (RF06, NEG06, NEG07)
/// </summary>
public interface IAvaliacaoService
{
    Task<Avaliacao?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Avaliacao?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default);
    Task<Avaliacao> CriarAsync(Avaliacao avaliacao, CancellationToken cancellationToken = default);
    Task<IEnumerable<Avaliacao>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Avaliacao>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<bool> PodeAvaliarAsync(Guid clienteId, Guid agendamentoId, CancellationToken cancellationToken = default);
}
