namespace HomeTask.Domain.Entidades;

/// <summary>
/// Avaliação de prestador por cliente (RF06, NEG06, NEG07)
/// </summary>
public class Avaliacao
{
    public Avaliacao() { }

    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid AgendamentoId { get; private set; }

    public Agendamento Agendamento { get; private set; } = null!;

    public Guid ClienteId { get; private set; }

    public Cliente Cliente { get; private set; } = null!;

    public Guid PrestadorId { get; private set; }

    public Prestador Prestador { get; private set; } = null!;

    public Guid ServicoPrestadorId { get; private set; }

    public ServicoPrestador ServicoPrestador { get; private set; } = null!;

    /// <summary>
    /// Nota de 0 a 5 estrelas para o servico executado
    /// </summary>
    public int NotaServico { get; private set; }

    /// <summary>
    /// Nota de 0 a 5 estrelas para o prestador
    /// </summary>
    public int NotaPrestador { get; private set; }

    /// <summary>
    /// Comentário opcional sobre o serviço
    /// </summary>
    public string? Comentario { get; private set; }

    public DateTime DataAvaliacao { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Avaliações são públicas e permanentes (NEG07)
    /// </summary>
    public bool Visivel { get; private set; } = true;

    public void DefinirDados(
        Guid id,
        Guid agendamentoId,
        Guid clienteId,
        Guid prestadorId,
        Guid servicoPrestadorId,
        int notaServico,
        int notaPrestador,
        string? comentario,
        DateTime dataAvaliacao,
        bool visivel)
    {
        Id = id;
        AgendamentoId = agendamentoId;
        ClienteId = clienteId;
        PrestadorId = prestadorId;
        ServicoPrestadorId = servicoPrestadorId;
        NotaServico = notaServico;
        NotaPrestador = notaPrestador;
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
