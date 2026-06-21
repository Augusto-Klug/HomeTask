namespace HomeTask.Domain.Entidades;

public class AvaliacaoCliente
{
    public AvaliacaoCliente() { }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid AgendamentoId { get; private set; }
    public Agendamento Agendamento { get; private set; } = null!;
    public Guid ClienteId { get; private set; }
    public Cliente Cliente { get; private set; } = null!;
    public Guid PrestadorId { get; private set; }
    public Prestador Prestador { get; private set; } = null!;
    public int Nota { get; private set; }
    public string? Comentario { get; private set; }
    public DateTime DataAvaliacao { get; private set; } = DateTime.UtcNow;
    public bool Visivel { get; private set; } = true;

    public void DefinirDados(
        Guid id,
        Guid agendamentoId,
        Guid clienteId,
        Guid prestadorId,
        int nota,
        string? comentario,
        DateTime dataAvaliacao,
        bool visivel)
    {
        Id = id;
        AgendamentoId = agendamentoId;
        ClienteId = clienteId;
        PrestadorId = prestadorId;
        Nota = nota;
        Comentario = comentario;
        DataAvaliacao = dataAvaliacao;
        Visivel = visivel;
    }

    public void Publicar(DateTime dataAvaliacao)
    {
        DataAvaliacao = dataAvaliacao;
        Visivel = true;
    }
}
