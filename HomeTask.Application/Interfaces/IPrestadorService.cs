using HomeTask.Application.Dtos;
using HomeTask.Domain.Enums;

namespace HomeTask.Application.Interfaces;
public interface IPrestadorService
{
    Task<PrestadorDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PrestadorDto?> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<PrestadorDto> CriarAsync(PrestadorDto prestador, CancellationToken cancellationToken = default);
    Task<PrestadorDto> AtualizarAsync(PrestadorDto prestador, CancellationToken cancellationToken = default);
    Task<IEnumerable<PrestadorDto>> BuscarAsync(CategoriaServico? categoria, string? cidade, DateTime? dataDisponivel, CancellationToken cancellationToken = default);
    Task<IEnumerable<AgendamentoResumoDto>> ObterHistoricoServicosAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task<PrestadorRecebimentosResumoDto> ObterRecebimentosAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task AtualizarMediaAvaliacoesAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task AtualizarStatusAsync(Guid prestadorId, StatusPrestador status, CancellationToken cancellationToken = default);
    Task<int> ProcessarSuspensoesExpiradasAsync(CancellationToken cancellationToken = default);
    Task<PrestadorPerfilPublicoDto?> ObterPerfilPublicoAsync(Guid prestadorId, CancellationToken cancellationToken = default);
}
