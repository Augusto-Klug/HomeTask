using HomeTask.Domain.Entities;
using HomeTask.Domain.Enums;

namespace HomeTask.Application.Interfaces;

/// <summary>
/// Interface para gerenciamento de agendamentos (RF04, RF10)
/// </summary>
public interface IAgendamentoService
{
    Task<Agendamento?> ObterPorIdAsync(Guid id);
    Task<Agendamento> CriarAsync(Agendamento agendamento);
    Task<Agendamento> AceitarAsync(Guid agendamentoId);
    Task<Agendamento> RecusarAsync(Guid agendamentoId, string motivo);
    Task<Agendamento> IniciarAsync(Guid agendamentoId);
    Task<Agendamento> ConcluirAsync(Guid agendamentoId);
    Task<Agendamento> CancelarAsync(Guid agendamentoId, string motivo);
    Task<IEnumerable<Agendamento>> ObterPorClienteAsync(Guid clienteId);
    Task<IEnumerable<Agendamento>> ObterPorPrestadorAsync(Guid prestadorId);
    Task<IEnumerable<Agendamento>> ObterPorStatusAsync(StatusAgendamento status);
}
