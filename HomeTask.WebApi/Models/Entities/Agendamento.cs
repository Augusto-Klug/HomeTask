using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HomeTask.WebApi.Models.Enums;

namespace HomeTask.WebApi.Models.Entities;

/// <summary>
/// Agendamento de serviço entre cliente e prestador (RF04, RF10)
/// </summary>
public class Agendamento
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid ClienteId { get; set; }

    [ForeignKey(nameof(ClienteId))]
    public Cliente Cliente { get; set; } = null!;

    [Required]
    public Guid PrestadorId { get; set; }

    [ForeignKey(nameof(PrestadorId))]
    public Prestador Prestador { get; set; } = null!;

    [Required]
    public Guid ServicoOferecidoId { get; set; }

    [ForeignKey(nameof(ServicoOferecidoId))]
    public ServicoOferecido ServicoOferecido { get; set; } = null!;

    /// <summary>
    /// Data e hora agendada para o serviço
    /// </summary>
    [Required]
    public DateTime DataHoraAgendada { get; set; }

    /// <summary>
    /// Duração estimada em minutos
    /// </summary>
    public int DuracaoMinutos { get; set; }

    /// <summary>
    /// Status atual do agendamento
    /// </summary>
    public StatusAgendamento Status { get; set; } = StatusAgendamento.Solicitado;

    /// <summary>
    /// Endereço de realização do serviço
    /// </summary>
    [MaxLength(300)]
    public string? EnderecoServico { get; set; }

    [MaxLength(500)]
    public string? Observacoes { get; set; }

    /// <summary>
    /// Valor total do serviço
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal ValorTotal { get; set; }

    public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;

    public DateTime? DataResposta { get; set; }

    public DateTime? DataConclusao { get; set; }

    [MaxLength(500)]
    public string? MotivoRecusa { get; set; }

    // Navegação
    public Pagamento? Pagamento { get; set; }
    public Avaliacao? Avaliacao { get; set; }
}
