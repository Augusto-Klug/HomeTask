using HomeTask.WebApi.Models.Entities;
using HomeTask.WebApi.Models.Enums;

namespace HomeTask.WebApi.Services.Interfaces;

/// <summary>
/// Interface para gerenciamento de pagamentos (RF05, NEG04)
/// </summary>
public interface IPagamentoService
{
    Task<Pagamento?> ObterPorIdAsync(Guid id);
    Task<Pagamento?> ObterPorAgendamentoAsync(Guid agendamentoId);
    Task<Pagamento> CriarAsync(Pagamento pagamento);
    Task<Pagamento> ProcessarAsync(Guid pagamentoId);
    Task<Pagamento> ConfirmarAsync(Guid pagamentoId, string transacaoId);
    Task<Pagamento> RecusarAsync(Guid pagamentoId, string motivo);
    Task<Pagamento> EstornarAsync(Guid pagamentoId);
}
