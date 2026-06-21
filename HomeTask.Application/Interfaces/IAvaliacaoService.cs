using HomeTask.Application.Dtos;

namespace HomeTask.Application.Interfaces;

/// <summary>
/// Interface para gerenciamento de avaliações (RF06, NEG06, NEG07)
/// </summary>
public interface IAvaliacaoService
{
    Task<AvaliacaoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AvaliacaoDto?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default);
    Task<AvaliacaoDto> CriarAsync(AvaliacaoDto avaliacao, CancellationToken cancellationToken = default);
    Task<IEnumerable<AvaliacaoDto>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AvaliacaoDto>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<bool> PodeAvaliarAsync(Guid clienteId, Guid agendamentoId, CancellationToken cancellationToken = default);
}

public interface IAvaliacaoClienteService
{
    Task<AvaliacaoClienteDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AvaliacaoClienteDto?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default);
    Task<AvaliacaoClienteDto> CriarAsync(AvaliacaoClienteDto avaliacao, CancellationToken cancellationToken = default);
    Task<IEnumerable<AvaliacaoClienteDto>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AvaliacaoClienteDto>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task<bool> PodeAvaliarAsync(Guid prestadorId, Guid agendamentoId, CancellationToken cancellationToken = default);
}
