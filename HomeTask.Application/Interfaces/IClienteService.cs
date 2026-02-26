using HomeTask.Domain.Entities;

namespace HomeTask.Application.Interfaces;
public interface IClienteService
{
    Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Cliente?> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<Cliente> CriarAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task<Cliente> AtualizarAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ObterHistoricoAgendamentosAsync(Guid clienteId, CancellationToken cancellationToken = default);
}
