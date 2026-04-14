using HomeTask.Domain.Entities;
using HomeTask.Domain.Enums;

namespace HomeTask.Application.Interfaces;
public interface IPrestadorService
{
    Task<Prestador?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Prestador?> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<Prestador> CriarAsync(Prestador prestador, CancellationToken cancellationToken = default);
    Task<Prestador> AtualizarAsync(Prestador prestador, CancellationToken cancellationToken = default);
    //Task<IEnumerable<Prestador>> BuscarAsync(CategoriaServico? categoria, string? cidade, DateTime? dataDisponivel, CancellationToken cancellationToken = default);
    Task<IEnumerable<Prestador>> BuscarAsync(Guid? categoriaId, string? cidade, DateTime? dataDisponivel, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ObterHistoricoServicosAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task AtualizarMediaAvaliacoesAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task AtualizarStatusAsync(Guid prestadorId, StatusPrestador status, CancellationToken cancellationToken = default);
}
