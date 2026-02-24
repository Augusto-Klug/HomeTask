using HomeTask.Domain.Entities;

namespace HomeTask.Application.Interfaces;

/// <summary>
/// Interface para chat entre usuários (RF08, NEG09)
/// </summary>
public interface IMensagemService
{
    Task<Mensagem?> ObterPorIdAsync(Guid id);
    Task<Mensagem> EnviarAsync(Mensagem mensagem);
    Task<IEnumerable<Mensagem>> ObterConversaAsync(Guid usuarioId1, Guid usuarioId2);
    Task<IEnumerable<Mensagem>> ObterConversasPorUsuarioAsync(Guid usuarioId);
    Task MarcarComoLidaAsync(Guid mensagemId);
    Task<int> ObterNaoLidasAsync(Guid usuarioId);
}
