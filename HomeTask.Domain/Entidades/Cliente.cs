using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Entities;

/// <summary>
/// Entidade para clientes que contratam serviços domésticos
/// </summary>
public class Cliente
{
    public Cliente() { }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UsuarioId { get; private set; }
    public Usuario Usuario { get; private set; } = null!;

    // Navegação
    public ICollection<Agendamento> Agendamentos { get; private set; } = [];
    public ICollection<Avaliacao> Avaliacoes { get; private set; } = [];
    public ICollection<Conversa> Conversas { get; private set; } = [];

    public void DefinirDados(Guid id, Guid usuarioId)
    {
        Id = id;
        UsuarioId = usuarioId;
    }
}
