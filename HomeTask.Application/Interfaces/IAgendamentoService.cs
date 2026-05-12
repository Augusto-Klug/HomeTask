using HomeTask.Application.Dtos;
using HomeTask.Domain.Enums;

namespace HomeTask.Application.Interfaces;

/// <summary>
/// Interface para gerenciamento de agendamentos (RF04, RF10)
/// </summary>
public interface IAgendamentoService
{
    Task<AgendamentoResumoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AgendamentoDto> CriarAsync(AgendamentoDto agendamento, CancellationToken cancellationToken = default);
    Task<AgendamentoDto> AceitarAsync(Guid agendamentoId, CancellationToken cancellationToken = default);
    Task<AgendamentoDto> RecusarAsync(Guid agendamentoId, string motivo, CancellationToken cancellationToken = default);
    Task<AgendamentoDto> IniciarAsync(Guid agendamentoId, CancellationToken cancellationToken = default);
    Task<AgendamentoDto> ConcluirAsync(Guid agendamentoId, CancellationToken cancellationToken = default);
    Task<AgendamentoDto> CancelarAsync(Guid agendamentoId, string motivo, CancellationToken cancellationToken = default);
    Task<IEnumerable<AgendamentoResumoDto>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AgendamentoResumoDto>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AgendamentoResumoDto>> ObterSolicitacoesPendentesPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AgendamentoResumoDto>> ObterPorStatusAsync(StatusAgendamento status, CancellationToken cancellationToken = default);
}
