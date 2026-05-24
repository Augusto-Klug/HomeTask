namespace HomeTask.Domain.Entidades;

/// <summary>
/// Disponibilidade de horários do prestador (NEG05)
/// </summary>
public class Disponibilidade
{
    public Disponibilidade() { }

    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid PrestadorId { get; private set; }

    public Prestador Prestador { get; private set; } = null!;

    /// <summary>
    /// Dia da semana (0 = Domingo, 6 = Sábado)
    /// </summary>
    public int DiaSemana { get; private set; }

    /// <summary>
    /// Hora de início da disponibilidade
    /// </summary>
    public TimeSpan HoraInicio { get; private set; }

    /// <summary>
    /// Hora de fim da disponibilidade
    /// </summary>
    public TimeSpan HoraFim { get; private set; }

    public bool Ativo { get; private set; } = true;
}
