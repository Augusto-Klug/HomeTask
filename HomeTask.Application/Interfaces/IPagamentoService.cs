using HomeTask.Application.Dtos;

namespace HomeTask.Application.Interfaces;

/// <summary>
/// Interface para gerenciamento de pagamentos (RF05, NEG04)
/// </summary>
public interface IPagamentoService
{
    Task<PagamentoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagamentoDto?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default);
    Task<PagamentoDto> CriarAsync(PagamentoDto pagamento, CancellationToken cancellationToken = default);
    Task<PagamentoDto> ProcessarAsync(Guid pagamentoId, CancellationToken cancellationToken = default);
    Task<PagamentoDto> ConfirmarAsync(Guid pagamentoId, string transacaoId, CancellationToken cancellationToken = default);
    Task<PagamentoDto> RecusarAsync(Guid pagamentoId, string motivo, CancellationToken cancellationToken = default);
    Task<PagamentoDto> EstornarAsync(Guid pagamentoId, CancellationToken cancellationToken = default);
}
