using HomeTask.WebApi.Models.Entities;
using HomeTask.WebApi.Models.Enums;

namespace HomeTask.WebApi.Services.Interfaces;

/// <summary>
/// Interface para gerenciamento de pagamentos (RF05, NEG04)
/// </summary>
public interface IPagamentoService
{
    Task<Pagamento?> ObterPorIdAsync(int id);
    Task<Pagamento?> ObterPorAgendamentoAsync(int agendamentoId);
    Task<Pagamento> CriarAsync(Pagamento pagamento);
    Task<Pagamento> ProcessarAsync(int pagamentoId);
    Task<Pagamento> ConfirmarAsync(int pagamentoId, string transacaoId);
    Task<Pagamento> RecusarAsync(int pagamentoId, string motivo);
    Task<Pagamento> EstornarAsync(int pagamentoId);
}
