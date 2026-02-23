using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeTask.WebApi.Models.Entities;

/// <summary>
/// Disponibilidade de horários do prestador (NEG05)
/// </summary>
public class Disponibilidade
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid PrestadorId { get; set; }

    [ForeignKey(nameof(PrestadorId))]
    public Prestador Prestador { get; set; } = null!;

    /// <summary>
    /// Dia da semana (0 = Domingo, 6 = Sábado)
    /// </summary>
    [Required]
    [Range(0, 6)]
    public int DiaSemana { get; set; }

    /// <summary>
    /// Hora de início da disponibilidade
    /// </summary>
    [Required]
    public TimeSpan HoraInicio { get; set; }

    /// <summary>
    /// Hora de fim da disponibilidade
    /// </summary>
    [Required]
    public TimeSpan HoraFim { get; set; }

    public bool Ativo { get; set; } = true;
}
