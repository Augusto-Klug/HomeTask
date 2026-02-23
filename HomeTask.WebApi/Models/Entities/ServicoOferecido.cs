using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HomeTask.WebApi.Models.Enums;

namespace HomeTask.WebApi.Models.Entities;

/// <summary>
/// Serviço oferecido por um prestador (RF02, RF03)
/// </summary>
public class ServicoOferecido
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PrestadorId { get; set; }

    [ForeignKey(nameof(PrestadorId))]
    public Prestador Prestador { get; set; } = null!;

    [Required]
    public CategoriaServico Categoria { get; set; }

    [MaxLength(100)]
    public string? Titulo { get; set; }

    [MaxLength(500)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Preço base do serviço
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecoBase { get; set; }

    /// <summary>
    /// Unidade de cobrança (hora, diária, serviço)
    /// </summary>
    [MaxLength(20)]
    public string UnidadeCobranca { get; set; } = "hora";

    /// <summary>
    /// Duração estimada em minutos
    /// </summary>
    public int DuracaoEstimadaMinutos { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}
