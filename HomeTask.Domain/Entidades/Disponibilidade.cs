namespace HomeTask.Domain.Entities;

/// <summary>
/// Disponibilidade de horários do prestador (NEG05)
/// </summary>
public class Disponibilidade
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PrestadorId { get; set; }

    public Prestador Prestador { get; set; } = null!;

    /// <summary>
    /// Dia da semana (0 = Domingo, 6 = Sábado)
    /// </summary>
    public int DiaSemana { get; set; }

    /// <summary>
    /// Hora de início da disponibilidade
    /// </summary>
    public TimeSpan HoraInicio { get; set; }

    /// <summary>
    /// Hora de fim da disponibilidade
    /// </summary>
    public TimeSpan HoraFim { get; set; }

    public bool Ativo { get; set; } = true;
}
