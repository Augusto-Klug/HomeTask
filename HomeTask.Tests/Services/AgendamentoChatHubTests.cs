using System.Security.Claims;
using HomeTask.Application.Dtos;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Enums;
using HomeTask.WebApi.Hubs;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace HomeTask.Tests.Services;

public class AgendamentoChatHubTests
{
    [Fact]
    public async Task EntrarNoAgendamento_QuandoUsuarioParticipa_DeveAdicionarConexaoAoGrupoDoAgendamento()
    {
        var contexto = CriarContextoUsuario(Guid.NewGuid());
        var agendamentoId = Guid.NewGuid();
        var groups = new FakeGroupManager();
        var hub = CriarHub(contexto, groups: groups, agendamentoId: agendamentoId, clienteUsuarioId: contexto.UsuarioId);

        await hub.EntrarNoAgendamento(agendamentoId);

        Assert.Equal(contexto.ConnectionId, groups.ConnectionIdAdicionada);
        Assert.Equal($"agendamento:{agendamentoId}", groups.GrupoAdicionado);
    }

    [Fact]
    public async Task EnviarMensagem_DevePersistirEEnviarMensagemParaTodosDoGrupoDoAgendamento()
    {
        var contexto = CriarContextoUsuario(Guid.NewGuid());
        var agendamentoId = Guid.NewGuid();
        var clients = new FakeHubCallerClients();
        var mensagemService = new FakeMensagemService();
        var hub = CriarHub(
            contexto,
            clients: clients,
            agendamentoId: agendamentoId,
            clienteUsuarioId: contexto.UsuarioId,
            mensagemService: mensagemService);

        await hub.EnviarMensagem(agendamentoId, "Olá");

        Assert.Equal(agendamentoId, mensagemService.UltimoAgendamentoId);
        Assert.Equal(contexto.UsuarioId, mensagemService.UltimoRemetenteId);
        Assert.Equal($"agendamento:{agendamentoId}", clients.UltimoGrupo);
        Assert.Equal("MensagemRecebida", clients.Proxy.UltimoMetodo);
        Assert.Single(clients.Proxy.UltimosArgumentos);
        Assert.IsType<MensagemDto>(clients.Proxy.UltimosArgumentos[0]);
    }

    private static AgendamentoChatHub CriarHub(
        FakeHubCallerContext contexto,
        Guid agendamentoId,
        Guid clienteUsuarioId,
        FakeHubCallerClients? clients = null,
        FakeGroupManager? groups = null,
        FakeMensagemService? mensagemService = null)
    {
        var agendamento = new AgendamentoResumoDto
        {
            Id = agendamentoId,
            ClienteId = Guid.NewGuid(),
            PrestadorId = Guid.NewGuid(),
            Status = StatusAgendamento.Aceito,
        };
        var cliente = new ClienteDto { Id = agendamento.ClienteId, UsuarioId = clienteUsuarioId };

        return new AgendamentoChatHub(
            new FakeAgendamentoService(agendamento),
            new FakeClienteService(cliente),
            new FakePrestadorService(null),
            mensagemService ?? new FakeMensagemService(),
            new MemoryCache(new MemoryCacheOptions()))
        {
            Context = contexto,
            Clients = clients ?? new FakeHubCallerClients(),
            Groups = groups ?? new FakeGroupManager(),
        };
    }

    private static FakeHubCallerContext CriarContextoUsuario(Guid usuarioId) =>
        new(usuarioId, $"conn-{Guid.NewGuid()}");

    private sealed class FakeHubCallerContext : HubCallerContext
    {
        public FakeHubCallerContext(Guid usuarioId, string connectionId)
        {
            UsuarioId = usuarioId;
            ConnectionId = connectionId;
            User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString())], "Test"));
        }

        public Guid UsuarioId { get; }
        public override string ConnectionId { get; }
        public override string? UserIdentifier => UsuarioId.ToString();
        public override ClaimsPrincipal? User { get; }
        public override IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();
        public override IFeatureCollection Features { get; } = new FeatureCollection();
        public override CancellationToken ConnectionAborted { get; } = CancellationToken.None;
        public override void Abort() { }
    }

    private sealed class FakeGroupManager : IGroupManager
    {
        public string? ConnectionIdAdicionada { get; private set; }
        public string? GrupoAdicionado { get; private set; }

        public Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
        {
            ConnectionIdAdicionada = connectionId;
            GrupoAdicionado = groupName;
            return Task.CompletedTask;
        }

        public Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class FakeHubCallerClients : IHubCallerClients
    {
        public FakeClientProxy Proxy { get; } = new();
        public string? UltimoGrupo { get; private set; }
        public IClientProxy All => Proxy;
        public IClientProxy Caller => Proxy;
        public IClientProxy Others => Proxy;
        public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) => Proxy;
        public IClientProxy Client(string connectionId) => Proxy;
        public IClientProxy Clients(IReadOnlyList<string> connectionIds) => Proxy;
        public IClientProxy Group(string groupName)
        {
            UltimoGrupo = groupName;
            return Proxy;
        }
        public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds) => Group(groupName);
        public IClientProxy Groups(IReadOnlyList<string> groupNames) => Proxy;
        public IClientProxy OthersInGroup(string groupName) => Group(groupName);
        public IClientProxy User(string userId) => Proxy;
        public IClientProxy Users(IReadOnlyList<string> userIds) => Proxy;
    }

    private sealed class FakeClientProxy : IClientProxy
    {
        public string? UltimoMetodo { get; private set; }
        public object?[] UltimosArgumentos { get; private set; } = [];

        public Task SendCoreAsync(string method, object?[] args, CancellationToken cancellationToken = default)
        {
            UltimoMetodo = method;
            UltimosArgumentos = args;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeAgendamentoService : IAgendamentoService
    {
        private readonly AgendamentoResumoDto _agendamento;
        public FakeAgendamentoService(AgendamentoResumoDto agendamento) => _agendamento = agendamento;
        public Task<AgendamentoResumoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<AgendamentoResumoDto?>(_agendamento);
        public Task<AgendamentoDto> CriarAsync(AgendamentoDto agendamento, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<AgendamentoDto> AceitarAsync(Guid agendamentoId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<AgendamentoDto> RecusarAsync(Guid agendamentoId, string motivo, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<AgendamentoDto> IniciarAsync(Guid agendamentoId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<AgendamentoDto> ConcluirAsync(Guid agendamentoId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<AgendamentoDto> CancelarAsync(Guid agendamentoId, string motivo, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IEnumerable<AgendamentoResumoDto>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IEnumerable<AgendamentoResumoDto>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IEnumerable<AgendamentoResumoDto>> ObterSolicitacoesPendentesPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IEnumerable<AgendamentoResumoDto>> ObterPorStatusAsync(StatusAgendamento status, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }

    private sealed class FakeClienteService : IClienteService
    {
        private readonly ClienteDto? _cliente;
        public FakeClienteService(ClienteDto? cliente) => _cliente = cliente;
        public Task<ClienteDto?> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default) => Task.FromResult(_cliente?.UsuarioId == usuarioId ? _cliente : null);
        public Task<ClienteDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<ClienteDto> CriarAsync(ClienteDto cliente, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<ClienteDto> AtualizarAsync(ClienteDto cliente, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IEnumerable<AgendamentoResumoDto>> ObterHistoricoAgendamentosAsync(Guid clienteId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }

    private sealed class FakePrestadorService : IPrestadorService
    {
        private readonly PrestadorDto? _prestador;
        public FakePrestadorService(PrestadorDto? prestador) => _prestador = prestador;
        public Task<PrestadorDto?> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default) => Task.FromResult(_prestador?.UsuarioId == usuarioId ? _prestador : null);
        public Task<PrestadorDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<PrestadorDto> CriarAsync(PrestadorDto prestador, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<PrestadorDto> AtualizarAsync(PrestadorDto prestador, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IEnumerable<PrestadorDto>> BuscarAsync(CategoriaServico? categoria, string? cidade, DateTime? dataDisponivel, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IEnumerable<AgendamentoResumoDto>> ObterHistoricoServicosAsync(Guid prestadorId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task AtualizarMediaAvaliacoesAsync(Guid prestadorId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task AtualizarStatusAsync(Guid prestadorId, StatusPrestador status, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<PrestadorPerfilPublicoDto?> ObterPerfilPublicoAsync(Guid prestadorId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }

    private sealed class FakeMensagemService : IMensagemService
    {
        public Guid? UltimoAgendamentoId { get; private set; }
        public Guid? UltimoRemetenteId { get; private set; }

        public Task<MensagemDto> EnviarPorAgendamentoAsync(Guid agendamentoId, Guid remetenteId, string conteudo, CancellationToken cancellationToken = default)
        {
            UltimoAgendamentoId = agendamentoId;
            UltimoRemetenteId = remetenteId;
            return Task.FromResult(new MensagemDto
            {
                Id = Guid.NewGuid(),
                AgendamentoId = agendamentoId,
                RemetenteId = remetenteId,
                Conteudo = conteudo,
                DataEnvio = DateTime.UtcNow,
            });
        }

        public Task<MensagemDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<MensagemDto> EnviarAsync(MensagemDto mensagem, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IEnumerable<MensagemDto>> ObterConversaAsync(Guid conversaId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IEnumerable<MensagemDto>> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IEnumerable<MensagemDto>> ObterConversasPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task MarcarComoLidaAsync(Guid mensagemId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<int> ObterNaoLidasAsync(Guid usuarioId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<int> RemoverExpiradasAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }
}
