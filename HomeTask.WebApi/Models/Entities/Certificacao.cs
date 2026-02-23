using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeTask.WebApi.Models.Entities;

/// <summary>
/// Certificação do prestador de serviço (RF02)
/// </summary>
public class Certificacao
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PrestadorId { get; set; }

    [ForeignKey(nameof(PrestadorId))]
    public Prestador Prestador { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Instituicao { get; set; }

    public DateTime? DataEmissao { get; set; }

    public DateTime? DataValidade { get; set; }

    /// <summary>
    /// URL do documento/certificado
    /// </summary>
    [MaxLength(500)]
    public string? UrlDocumento { get; set; }

    public bool Verificada { get; set; } = false;

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}
