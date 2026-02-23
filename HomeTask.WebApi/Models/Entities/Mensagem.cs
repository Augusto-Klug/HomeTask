using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeTask.WebApi.Models.Entities;

/// <summary>
/// Mensagem do chat entre cliente e prestador (RF08, NEG09)
/// </summary>
public class Mensagem
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Usuário que enviou a mensagem
    /// </summary>
    [Required]
    public int RemetenteId { get; set; }

    [ForeignKey(nameof(RemetenteId))]
    public Usuario Remetente { get; set; } = null!;

    /// <summary>
    /// Usuário que recebeu a mensagem
    /// </summary>
    [Required]
    public int DestinatarioId { get; set; }

    [ForeignKey(nameof(DestinatarioId))]
    public Usuario Destinatario { get; set; } = null!;

    /// <summary>
    /// Agendamento relacionado à conversa (opcional)
    /// </summary>
    public int? AgendamentoId { get; set; }

    [ForeignKey(nameof(AgendamentoId))]
    public Agendamento? Agendamento { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Conteudo { get; set; } = string.Empty;

    public DateTime DataEnvio { get; set; } = DateTime.UtcNow;

    public DateTime? DataLeitura { get; set; }

    public bool Lida { get; set; } = false;
}
