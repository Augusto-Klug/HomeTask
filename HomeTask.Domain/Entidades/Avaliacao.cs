namespace HomeTask.Domain.Entities;

/// <summary>
/// Avaliação de prestador por cliente (RF06, NEG06, NEG07)
/// </summary>
public class Avaliacao
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AgendamentoId { get; set; }

    public Agendamento Agendamento { get; set; } = null!;

    public Guid ClienteId { get; set; }

    public Cliente Cliente { get; set; } = null!;

    public Guid PrestadorId { get; set; }

    public Prestador Prestador { get; set; } = null!;

    /// <summary>
    /// Nota de 0 a 5 estrelas (NEG07)
    /// </summary>
    public int Nota { get; set; }

    /// <summary>
    /// Comentário opcional sobre o serviço
    /// </summary>
    public string? Comentario { get; set; }

    public DateTime DataAvaliacao { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Avaliações são públicas e permanentes (NEG07)
    /// </summary>
    public bool Visivel { get; set; } = true;
}
