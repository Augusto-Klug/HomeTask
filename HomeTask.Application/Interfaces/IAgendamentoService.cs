using HomeTask.Domain.Entities;
using HomeTask.Domain.Enums;

namespace HomeTask.Application.Interfaces;

/// <summary>
/// Interface para gerenciamento de agendamentos (RF04, RF10)
/// </summary>
public interface IAgendamentoService
{
    Task<Agendamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Agendamento> CriarAsync(Agendamento agendamento, List<Guid> servicosIds, CancellationToken cancellationToken = default);
    Task<Agendamento> AceitarAsync(Guid agendamentoId, CancellationToken cancellationToken = default);
    Task<Agendamento> RecusarAsync(Guid agendamentoId, string motivo, CancellationToken cancellationToken = default);
    Task<Agendamento> IniciarAsync(Guid agendamentoId, CancellationToken cancellationToken = default);
    Task<Agendamento> ConcluirAsync(Guid agendamentoId, CancellationToken cancellationToken = default);
    Task<Agendamento> CancelarAsync(Guid agendamentoId, string motivo, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ObterSolicitacoesPendentesPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ObterPorStatusAsync(StatusAgendamento status, CancellationToken cancellationToken = default);
}
