using HomeTask.Domain.Entidades;
using HomeTask.Domain.Entities;

namespace HomeTask.Application.Interfaces;

public interface IConversaService
{
    Task<Conversa?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Conversa>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna a conversa existente entre cliente e prestador,
    /// ou cria uma nova caso ainda não exista
    /// </summary>
    Task<Conversa> ObterOuCriarAsync(Guid clienteId, Guid prestadorId, CancellationToken cancellationToken = default);
}
