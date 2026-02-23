using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HomeTask.WebApi.Models.Enums;

namespace HomeTask.WebApi.Models.Entities;

/// <summary>
/// Entidade para prestadores de serviços domésticos (RF02)
/// </summary>
public class Prestador
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario Usuario { get; set; } = null!;

    [MaxLength(1000)]
    public string? Descricao { get; set; }

    // Endereço de atuação
    [MaxLength(200)]
    public string? Endereco { get; set; }

    [MaxLength(100)]
    public string? Cidade { get; set; }

    [MaxLength(50)]
    public string? Estado { get; set; }

    [MaxLength(10)]
    public string? Cep { get; set; }

    [MaxLength(100)]
    public string? Bairro { get; set; }

    /// <summary>
    /// Raio de atendimento em km
    /// </summary>
    public int RaioAtendimentoKm { get; set; } = 10;

    /// <summary>
    /// Status atual do prestador (NEG08)
    /// </summary>
    public StatusPrestador Status { get; set; } = StatusPrestador.EmAnalise;

    /// <summary>
    /// Média das avaliações recebidas (NEG07)
    /// </summary>
    [Column(TypeName = "decimal(3,2)")]
    public decimal MediaAvaliacoes { get; set; } = 0;

    /// <summary>
    /// Total de avaliações recebidas
    /// </summary>
    public int TotalAvaliacoes { get; set; } = 0;

    /// <summary>
    /// Total de serviços concluídos
    /// </summary>
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
