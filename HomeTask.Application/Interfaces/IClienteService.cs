using HomeTask.Application.Dtos;

namespace HomeTask.Application.Interfaces;
public interface IClienteService
{
    Task<ClienteDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ClienteDto?> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<ClienteDto> CriarAsync(ClienteDto cliente, CancellationToken cancellationToken = default);
    Task<ClienteDto> AtualizarAsync(ClienteDto cliente, CancellationToken cancellationToken = default);
    Task<IEnumerable<AgendamentoResumoDto>> ObterHistoricoAgendamentosAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task AtualizarMediaAvaliacoesAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<ClientePerfilPublicoDto?> ObterPerfilPublicoAsync(Guid clienteId, CancellationToken cancellationToken = default);
}
