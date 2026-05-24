using HomeTask.Application.Dtos;

namespace HomeTask.Application.Interfaces;

public interface IConversaService
{
    Task<ConversaDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<ConversaDto>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna a conversa existente entre cliente e prestador,
    /// ou cria uma nova caso ainda não exista
    /// </summary>
    Task<ConversaDto> ObterOuCriarAsync(Guid clienteId, Guid prestadorId, CancellationToken cancellationToken = default);
}
