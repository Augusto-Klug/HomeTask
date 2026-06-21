using HomeTask.Application.Dtos;

namespace HomeTask.Application.Interfaces;

/// <summary>
/// Interface para chat entre usuários (RF08, NEG09)
/// </summary>
public interface IMensagemService
{
    Task<MensagemDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MensagemDto> EnviarAsync(MensagemDto mensagem, CancellationToken cancellationToken = default);
    Task<MensagemDto> EnviarPorAgendamentoAsync(Guid agendamentoId, Guid remetenteId, string conteudo, CancellationToken cancellationToken = default);
    Task<IEnumerable<MensagemDto>> ObterConversaAsync(Guid conversaId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MensagemDto>> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MensagemDto>> ObterConversasPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task MarcarComoLidaAsync(Guid mensagemId, CancellationToken cancellationToken = default);
    Task<int> ObterNaoLidasAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<int> RemoverExpiradasAsync(CancellationToken cancellationToken = default);
}
