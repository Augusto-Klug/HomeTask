using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeTask.WebApi.Models.Entities;

/// <summary>
/// Entidade para clientes que contratam serviços domésticos
/// </summary>
public class Cliente
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario Usuario { get; set; } = null!;

    // Endereço principal
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
