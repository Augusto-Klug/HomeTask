using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HomeTask.WebApi.Models.Enums;

namespace HomeTask.WebApi.Models.Entities;

/// <summary>
/// Pagamento de serviço contratado (RF05, NEG04)
/// </summary>
public class Pagamento
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int AgendamentoId { get; set; }

    [ForeignKey(nameof(AgendamentoId))]
    public Agendamento Agendamento { get; set; } = null!;

    /// <summary>
    /// Valor do pagamento
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Valor { get; set; }

    /// <summary>
    /// Tipo de pagamento (Pix, Débito, Crédito)
    /// </summary>
    [Required]
    public TipoPagamento TipoPagamento { get; set; }

    /// <summary>
    /// Status do pagamento
    /// </summary>
    public StatusPagamento Status { get; set; } = StatusPagamento.Pendente;

    /// <summary>
    /// Identificador da transação no gateway de pagamento
    /// </summary>
    [MaxLength(100)]
    public string? TransacaoId { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public DateTime? DataProcessamento { get; set; }

    public DateTime? DataConfirmacao { get; set; }

    [MaxLength(500)]
    public string? MotivoRecusa { get; set; }
}
