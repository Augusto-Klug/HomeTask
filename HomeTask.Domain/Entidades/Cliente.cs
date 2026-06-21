namespace HomeTask.Domain.Entidades;

/// <summary>
/// Entidade para clientes que contratam serviços domésticos
/// </summary>
public class Cliente
{
    public Cliente() { }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UsuarioId { get; private set; }
    public Usuario Usuario { get; private set; } = null!;
    public decimal MediaAvaliacoes { get; private set; } = 0;
    public int TotalAvaliacoes { get; private set; } = 0;

    // Navegação
    public ICollection<ServicoCliente> ServicosClientes { get; private set; } = [];
    public ICollection<Agendamento> Agendamentos { get; private set; } = [];
    public ICollection<Avaliacao> Avaliacoes { get; private set; } = [];
    public ICollection<AvaliacaoCliente> AvaliacoesRecebidas { get; private set; } = [];
    public ICollection<Conversa> Conversas { get; private set; } = [];

    public void DefinirDados(Guid id, Guid usuarioId, decimal mediaAvaliacoes = 0, int totalAvaliacoes = 0)
    {
        Id = id;
        UsuarioId = usuarioId;
        MediaAvaliacoes = mediaAvaliacoes;
        TotalAvaliacoes = totalAvaliacoes;
    }

    public void AtualizarMetricasAvaliacao(decimal mediaAvaliacoes, int totalAvaliacoes)
    {
        MediaAvaliacoes = mediaAvaliacoes;
        TotalAvaliacoes = totalAvaliacoes;
    }
}
