using HomeTask.Domain.Entidades;

namespace HomeTask.Application.Interfaces;

/// <summary>
/// Interface para gerenciamento de pagamentos (RF05, NEG04)
/// </summary>
public interface IPagamentoService
{
    Task<Pagamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Pagamento?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default);
    Task<Pagamento> CriarAsync(Pagamento pagamento, CancellationToken cancellationToken = default);
    Task<Pagamento> ProcessarAsync(Guid pagamentoId, CancellationToken cancellationToken = default);
    Task<Pagamento> ConfirmarAsync(Guid pagamentoId, string transacaoId, CancellationToken cancellationToken = default);
    Task<Pagamento> RecusarAsync(Guid pagamentoId, string motivo, CancellationToken cancellationToken = default);
    Task<Pagamento> EstornarAsync(Guid pagamentoId, CancellationToken cancellationToken = default);
}
