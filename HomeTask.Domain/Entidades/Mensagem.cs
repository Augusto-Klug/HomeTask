using HomeTask.Domain.Entidades;

namespace HomeTask.Domain.Entities;

/// <summary>
/// Mensagem do chat entre cliente e prestador (RF08, NEG09)
/// </summary>
public class Mensagem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Usuário que enviou a mensagem
    /// </summary>
    public Guid RemetenteId { get; set; }

    public Usuario Remetente { get; set; } = null!;

    public Guid ConversaId { get; set; }
    public Conversa Conversa { get; set; } = null!;

    /// <summary>
    /// Agendamento relacionado à conversa (opcional)
    /// </summary>
    public Guid? AgendamentoId { get; set; }

    public Agendamento? Agendamento { get; set; }

    public string Conteudo { get; set; } = string.Empty;

    public DateTime DataEnvio { get; set; } = DateTime.UtcNow;

    public DateTime? DataLeitura { get; set; }

    public bool Lida { get; set; } = false;
}
