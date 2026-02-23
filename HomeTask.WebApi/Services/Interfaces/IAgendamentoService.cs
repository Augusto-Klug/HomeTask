using HomeTask.WebApi.Models.Entities;
using HomeTask.WebApi.Models.Enums;

namespace HomeTask.WebApi.Services.Interfaces;

/// <summary>
/// Interface para gerenciamento de agendamentos (RF04, RF10)
/// </summary>
public interface IAgendamentoService
{
    Task<Agendamento?> ObterPorIdAsync(int id);
    Task<Agendamento> CriarAsync(Agendamento agendamento);
    Task<Agendamento> AceitarAsync(int agendamentoId);
    Task<Agendamento> RecusarAsync(int agendamentoId, string motivo);
    Task<Agendamento> IniciarAsync(int agendamentoId);
    Task<Agendamento> ConcluirAsync(int agendamentoId);
    Task<Agendamento> CancelarAsync(int agendamentoId, string motivo);
    Task<IEnumerable<Agendamento>> ObterPorClienteAsync(int clienteId);
    Task<IEnumerable<Agendamento>> ObterPorPrestadorAsync(int prestadorId);
    Task<IEnumerable<Agendamento>> ObterPorStatusAsync(StatusAgendamento status);
}
