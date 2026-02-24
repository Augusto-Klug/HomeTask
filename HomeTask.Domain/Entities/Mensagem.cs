using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeTask.Domain.Entities;

/// <summary>
/// Mensagem do chat entre cliente e prestador (RF08, NEG09)
/// </summary>
public class Mensagem
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Usuário que enviou a mensagem
    /// </summary>
    [Required]
    public Guid RemetenteId { get; set; }

    [ForeignKey(nameof(RemetenteId))]
    public Usuario Remetente { get; set; } = null!;

    /// <summary>
    /// Usuário que recebeu a mensagem
    /// </summary>
    [Required]
    public Guid DestinatarioId { get; set; }

    [ForeignKey(nameof(DestinatarioId))]
    public Usuario Destinatario { get; set; } = null!;

    /// <summary>
    /// Agendamento relacionado à conversa (opcional)
    /// </summary>
    public Guid? AgendamentoId { get; set; }

    [ForeignKey(nameof(AgendamentoId))]
    public Agendamento? Agendamento { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Conteudo { get; set; } = string.Empty;

    public DateTime DataEnvio { get; set; } = DateTime.UtcNow;

    public DateTime? DataLeitura { get; set; }

    public bool Lida { get; set; } = false;
}
