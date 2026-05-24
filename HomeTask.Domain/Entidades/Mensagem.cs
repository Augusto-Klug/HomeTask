namespace HomeTask.Domain.Entidades;

/// <summary>
/// Mensagem do chat entre cliente e prestador (RF08, NEG09)
/// </summary>
public class Mensagem
{
    public Mensagem() { }

    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Usuário que enviou a mensagem
    /// </summary>
    public Guid RemetenteId { get; private set; }

    public Usuario Remetente { get; private set; } = null!;

    public Guid ConversaId { get; private set; }
    public Conversa Conversa { get; private set; } = null!;

    /// <summary>
    /// Agendamento relacionado à conversa (opcional)
    /// </summary>
    public Guid? AgendamentoId { get; private set; }

    public Agendamento? Agendamento { get; private set; }

    public string Conteudo { get; private set; } = string.Empty;

    public DateTime DataEnvio { get; private set; } = DateTime.UtcNow;

    public DateTime? DataLeitura { get; private set; }

    public bool Lida { get; private set; } = false;

    public void DefinirDados(
        Guid id,
        Guid remetenteId,
        Guid conversaId,
        Guid? agendamentoId,
        string conteudo,
        DateTime dataEnvio,
        DateTime? dataLeitura,
        bool lida)
    {
        Id = id;
        RemetenteId = remetenteId;
        ConversaId = conversaId;
        AgendamentoId = agendamentoId;
        Conteudo = conteudo;
        DataEnvio = dataEnvio;
        DataLeitura = dataLeitura;
        Lida = lida;
    }

    public void PrepararEnvio(DateTime dataEnvio)
    {
        DataEnvio = dataEnvio;
        Lida = false;
        DataLeitura = null;
    }

    public void MarcarComoLida(DateTime dataLeitura)
    {
        Lida = true;
        DataLeitura = dataLeitura;
    }
}
