using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeTask.WebApi.Models.Entities;

/// <summary>
/// Avaliação de prestador por cliente (RF06, NEG06, NEG07)
/// </summary>
public class Avaliacao
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid AgendamentoId { get; set; }

    [ForeignKey(nameof(AgendamentoId))]
    public Agendamento Agendamento { get; set; } = null!;

    [Required]
    public Guid ClienteId { get; set; }

    [ForeignKey(nameof(ClienteId))]
    public Cliente Cliente { get; set; } = null!;

    [Required]
    public Guid PrestadorId { get; set; }

    [ForeignKey(nameof(PrestadorId))]
    public Prestador Prestador { get; set; } = null!;

    /// <summary>
    /// Nota de 0 a 5 estrelas (NEG07)
    /// </summary>
    [Required]
    [Range(0, 5)]
    public int Nota { get; set; }

    /// <summary>
    /// Comentário opcional sobre o serviço
    /// </summary>
    [MaxLength(1000)]
    public string? Comentario { get; set; }

    public DateTime DataAvaliacao { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Avaliações são públicas e permanentes (NEG07)
    /// </summary>
    public bool Visivel { get; set; } = true;
}
