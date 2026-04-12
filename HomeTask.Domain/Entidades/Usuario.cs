using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Entities;
public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; }
    public string Email { get; set; } 
    public string SenhaHash { get; set; } 
    public string Documento { get; set; } 
    public string? Telefone { get; set; }
    public TipoUsuario TipoUsuario { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? UltimoAcesso { get; set; }
    public bool Ativo { get; set; } = true;

    // Navegação
    public Cliente? Cliente { get; set; }
    public Prestador? Prestador { get; set; }
    public ICollection<Endereco> Enderecos { get; set; } = [];
}
