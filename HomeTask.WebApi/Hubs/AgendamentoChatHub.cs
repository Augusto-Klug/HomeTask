using System.Security.Claims;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.SignalR;

namespace HomeTask.WebApi.Hubs;

[Authorize]
public class AgendamentoChatHub : Hub
{
    private const int LimiteMensagensPorJanela = 12;
    private static readonly TimeSpan JanelaRateLimit = TimeSpan.FromMinutes(1);

    private readonly IAgendamentoService _agendamentoService;
    private readonly IClienteService _clienteService;
    private readonly IPrestadorService _prestadorService;
    private readonly IMensagemService _mensagemService;
    private readonly IMemoryCache _memoryCache;

    public AgendamentoChatHub(
        IAgendamentoService agendamentoService,
        IClienteService clienteService,
        IPrestadorService prestadorService,
        IMensagemService mensagemService,
        IMemoryCache memoryCache)
    {
        _agendamentoService = agendamentoService;
        _clienteService = clienteService;
        _prestadorService = prestadorService;
        _mensagemService = mensagemService;
        _memoryCache = memoryCache;
    }

    public async Task EntrarNoAgendamento(Guid agendamentoId)
    {
        await ValidarAcessoAoChatAsync(agendamentoId, Context.ConnectionAborted);
        await Groups.AddToGroupAsync(Context.ConnectionId, ObterNomeGrupo(agendamentoId), Context.ConnectionAborted);
    }

    public async Task EnviarMensagem(Guid agendamentoId, string conteudo)
    {
        var usuarioId = await ValidarAcessoAoChatAsync(agendamentoId, Context.ConnectionAborted);

        if (ExcedeuRateLimit(usuarioId, agendamentoId))
            throw new HubException("Muitas mensagens em pouco tempo. Aguarde alguns instantes antes de tentar novamente.");

        try
        {
            var mensagem = await _mensagemService.EnviarPorAgendamentoAsync(
                agendamentoId,
                usuarioId,
                conteudo,
                Context.ConnectionAborted);

            await Clients.Group(ObterNomeGrupo(agendamentoId)).SendAsync("MensagemRecebida", mensagem, Context.ConnectionAborted);
        }
        catch (InvalidOperationException ex)
        {
            throw new HubException(ex.Message);
        }
    }

    private async Task<Guid> ValidarAcessoAoChatAsync(Guid agendamentoId, CancellationToken cancellationToken)
    {
        var usuarioId = ObterUsuarioId();
        if (usuarioId is null)
            throw new HubException("Usuario autenticado nao identificado.");

        var agendamento = await _agendamentoService.ObterPorIdAsync(agendamentoId, cancellationToken);
        if (agendamento is null)
            throw new HubException("Agendamento nao encontrado.");

        if (!StatusPermiteChat(agendamento.Status))
            throw new HubException("O chat fica disponivel apenas apos o aceite do agendamento.");

        var cliente = await _clienteService.ObterPorUsuarioIdAsync(usuarioId.Value, cancellationToken);
        if (cliente?.Id == agendamento.ClienteId)
            return usuarioId.Value;

        var prestador = await _prestadorService.ObterPorUsuarioIdAsync(usuarioId.Value, cancellationToken);
        if (prestador?.Id == agendamento.PrestadorId)
            return usuarioId.Value;

        throw new HubException("Usuario sem acesso a este chat.");
    }

    private Guid? ObterUsuarioId()
    {
        var claim = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Context.User?.FindFirstValue("sub");
        return Guid.TryParse(claim, out var usuarioId) ? usuarioId : null;
    }

    private bool ExcedeuRateLimit(Guid usuarioId, Guid agendamentoId)
    {
        var chave = $"chat-rate-limit:{usuarioId}:{agendamentoId}";
        var agora = DateTime.UtcNow;
        var limiteInferior = agora.Subtract(JanelaRateLimit);
        var fila = _memoryCache.GetOrCreate(chave, entry =>
        {
            entry.SlidingExpiration = JanelaRateLimit;
            return new Queue<DateTime>();
        })!;

        lock (fila)
        {
            while (fila.Count > 0 && fila.Peek() < limiteInferior)
                fila.Dequeue();

            if (fila.Count >= LimiteMensagensPorJanela)
                return true;

            fila.Enqueue(agora);
            return false;
        }
    }

    private static string ObterNomeGrupo(Guid agendamentoId) => $"agendamento:{agendamentoId}";

    private static bool StatusPermiteChat(StatusAgendamento status) =>
        status is StatusAgendamento.Aceito
            or StatusAgendamento.EmAndamento
            or StatusAgendamento.AguardandoPagamento
            or StatusAgendamento.Concluido;
}
