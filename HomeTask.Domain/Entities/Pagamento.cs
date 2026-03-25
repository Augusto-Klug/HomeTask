using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Entities;

/// <summary>
/// Pagamento de serviço contratado (RF05, NEG04)
/// </summary>
public class Pagamento
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AgendamentoId { get; set; }

    public Agendamento Agendamento { get; set; } = null!;

    /// <summary>
    /// Valor do pagamento
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Tipo de pagamento (Pix, Débito, Crédito)
    /// </summary>
    public TipoPagamento TipoPagamento { get; set; }

    /// <summary>
    /// Status do pagamento
    /// </summary>
    public StatusPagamento Status { get; set; } = StatusPagamento.Pendente;

    /// <summary>
    /// Identificador da transação no gateway de pagamento
    /// </summary>
    public string? TransacaoId { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public DateTime? DataProcessamento { get; set; }

    public DateTime? DataConfirmacao { get; set; }

    public string? MotivoRecusa { get; set; }
}
