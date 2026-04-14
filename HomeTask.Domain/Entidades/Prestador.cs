using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Entities;

/// <summary>
/// Entidade para prestadores de serviços domésticos (RF02)
/// </summary>
public class Prestador
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public string? Descricao { get; set; }
    public int? RaioAtendimentoKm { get; set; } = 10;
    public StatusPrestador Status { get; set; } = StatusPrestador.EmAnalise;
    public decimal MediaAvaliacoes { get; set; } = 0;
    public int TotalAvaliacoes { get; set; } = 0;
    public int TotalServicosConcluidos { get; set; } = 0;
    public DateTime? DataVerificacao { get; set; }

    // Navegação
    public ICollection<ServicoOferecido> ServicosOferecidos { get; set; } = [];
    public ICollection<Agendamento> Agendamentos { get; set; } = [];
    public ICollection<Avaliacao> Avaliacoes { get; set; } = [];
    public ICollection<Certificacao> Certificacoes { get; set; } = [];
    public ICollection<Portfolio> Portfolios { get; set; } = [];
    public ICollection<Disponibilidade> Disponibilidades { get; set; } = [];
    public ICollection<Conversa> Conversas { get; set; } = [];
}
