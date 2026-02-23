using HomeTask.WebApi.Models.Entities;

namespace HomeTask.WebApi.Services.Interfaces;

/// <summary>
/// Interface para chat entre usuários (RF08, NEG09)
/// </summary>
public interface IMensagemService
{
    Task<Mensagem?> ObterPorIdAsync(int id);
    Task<Mensagem> EnviarAsync(Mensagem mensagem);
    Task<IEnumerable<Mensagem>> ObterConversaAsync(int usuarioId1, int usuarioId2);
    Task<IEnumerable<Mensagem>> ObterConversasPorUsuarioAsync(int usuarioId);
    Task MarcarComoLidaAsync(int mensagemId);
    Task<int> ObterNaoLidasAsync(int usuarioId);
}
