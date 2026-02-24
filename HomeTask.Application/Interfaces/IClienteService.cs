using HomeTask.Domain.Entities;

namespace HomeTask.Application.Interfaces;

/// <summary>
/// Interface para gerenciamento de clientes (RF11)
/// </summary>
public interface IClienteService
{
    Task<Cliente?> ObterPorIdAsync(Guid id);
    Task<Cliente?> ObterPorUsuarioIdAsync(Guid usuarioId);
    Task<Cliente> CriarAsync(Cliente cliente);
    Task<Cliente> AtualizarAsync(Cliente cliente);
    Task<IEnumerable<Agendamento>> ObterHistoricoAgendamentosAsync(Guid clienteId);
}
