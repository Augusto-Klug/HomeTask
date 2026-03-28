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
    public TipoUsuario TipoUsuario { get; set; }
    public string Documento { get; set; }
    public string? Descricao { get; set; }
    public string Endereco { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }
    public string Cep { get; set; }
    public string Bairro { get; set; }
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
}
