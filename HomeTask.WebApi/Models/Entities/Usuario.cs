using System.ComponentModel.DataAnnotations;
using HomeTask.WebApi.Models.Enums;

namespace HomeTask.WebApi.Models.Entities;

/// <summary>
/// Entidade base para usuários da plataforma HomeTask (RF01)
/// </summary>
public class Usuario
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string SenhaHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(14)]
    public string Cpf { get; set; } = string.Empty;

    [MaxLength(15)]
    public string? Telefone { get; set; }

    [Required]
    public TipoUsuario Tipo { get; set; }

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public DateTime? UltimoAcesso { get; set; }

    public bool Ativo { get; set; } = true;

    // Navegação
    public Cliente? Cliente { get; set; }
    public Prestador? Prestador { get; set; }
}
