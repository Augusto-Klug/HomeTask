using HomeTask.Domain.Entities;

namespace HomeTask.Application.Interfaces;

/// <summary>
/// Interface para chat entre usuários (RF08, NEG09)
/// </summary>
public interface IMensagemService
{
    Task<Mensagem?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Mensagem> EnviarAsync(Mensagem mensagem, CancellationToken cancellationToken = default);
    Task<IEnumerable<Mensagem>> ObterConversaAsync(Guid conversaId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Mensagem>> ObterConversasPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task MarcarComoLidaAsync(Guid mensagemId, CancellationToken cancellationToken = default);
    Task<int> ObterNaoLidasAsync(Guid usuarioId, CancellationToken cancellationToken = default);
}
