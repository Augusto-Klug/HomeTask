using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeTask.Domain.Entities;

/// <summary>
/// Entidade para clientes que contratam serviços domésticos
/// </summary>
public class Cliente
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario Usuario { get; set; } = null!;

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

    // Navegação
    public ICollection<Agendamento> Agendamentos { get; set; } = [];
    public ICollection<Avaliacao> Avaliacoes { get; set; } = [];
}
