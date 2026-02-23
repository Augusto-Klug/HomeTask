using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeTask.WebApi.Models.Entities;

/// <summary>
/// Portfólio do prestador com fotos de trabalhos realizados (RF02)
/// </summary>
public class Portfolio
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid PrestadorId { get; set; }

    [ForeignKey(nameof(PrestadorId))]
    public Prestador Prestador { get; set; } = null!;

    [MaxLength(100)]
    public string? Titulo { get; set; }

    [MaxLength(500)]
    public string? Descricao { get; set; }

    /// <summary>
    /// URL da imagem
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string UrlImagem { get; set; } = string.Empty;

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public int Ordem { get; set; } = 0;
}
