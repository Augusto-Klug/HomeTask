using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Entities;

/// <summary>
/// Entidade base para usuários da plataforma HomeTask (RF01)
/// </summary>
public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Nome { get; set; }

    public string Email { get; set; } 

    public string SenhaHash { get; set; } 

    public string Documento { get; set; } 

    public string? Telefone { get; set; }

    public TipoUsuario Tipo { get; set; }

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public DateTime? UltimoAcesso { get; set; }

    public bool Ativo { get; set; } = true;

    // Navegação
    public Cliente? Cliente { get; set; }
    public Prestador? Prestador { get; set; }
}
