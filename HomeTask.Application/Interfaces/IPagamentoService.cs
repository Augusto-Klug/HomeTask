using HomeTask.Application.Dtos;

namespace HomeTask.Application.Interfaces;

/// <summary>
/// Interface para gerenciamento de pagamentos (RF05, NEG04)
/// </summary>
public interface IPagamentoService
{
    Task<PagamentoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagamentoDto?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default);
    Task<PagamentoDto> IniciarCheckoutAsync(Guid agendamentoId, Guid clienteId, CancellationToken cancellationToken = default);
    Task<PagamentoDto?> ProcessarWebhookAsync(PagamentoWebhookDto webhook, CancellationToken cancellationToken = default);
    Task<PagamentoDto?> ReconciliarPagamentoExternoAsync(string pagamentoExternoId, CancellationToken cancellationToken = default);
}

public interface IPagamentoGateway
{
    Task<PagamentoCheckoutResponseDto> CriarCheckoutPixAsync(PagamentoCheckoutRequestDto pagamento, CancellationToken cancellationToken = default);
    Task<PagamentoStatusGatewayDto?> ObterStatusPagamentoAsync(string pagamentoExternoId, CancellationToken cancellationToken = default);
}
