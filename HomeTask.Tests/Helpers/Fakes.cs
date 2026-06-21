using HomeTask.Domain.Common;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;
using HomeTask.Domain.Repositories;
using HomeTask.Application.Dtos;
using HomeTask.Application.Interfaces;

namespace HomeTask.Tests.Helpers;

internal abstract class FakeRepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected readonly Dictionary<Guid, T> Itens = [];
    public int AdicionarChamadas { get; private set; }
    public int AtualizarChamadas { get; private set; }
    public int RemoverChamadas { get; private set; }
    public int SalvarChamadas { get; private set; }
    public T? UltimoAdicionado { get; private set; }
    public T? UltimoAtualizado { get; private set; }

    public virtual Task<T?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Itens.GetValueOrDefault(id));

    public Task AdicionarAsync(T entidade, CancellationToken cancellationToken = default)
    {
        AdicionarChamadas++;
        UltimoAdicionado = entidade;
        Itens[ObterId(entidade)] = entidade;
        return Task.CompletedTask;
    }

    public void Atualizar(T entidade)
    {
        AtualizarChamadas++;
        UltimoAtualizado = entidade;
        Itens[ObterId(entidade)] = entidade;
    }

    public void Remover(T entidade)
    {
        RemoverChamadas++;
        Itens.Remove(ObterId(entidade));
    }

    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
    {
        SalvarChamadas++;
        return Task.CompletedTask;
    }

    public void Seed(params T[] entidades)
    {
        foreach (var entidade in entidades)
            Itens[ObterId(entidade)] = entidade;
    }

    private static Guid ObterId(T entidade) => (Guid)typeof(T).GetProperty("Id")!.GetValue(entidade)!;
}

internal sealed class FakeServicoPrestadorRepository : FakeRepositoryBase<ServicoPrestador>, IServicoPrestadorRepository
{
    public Task<IEnumerable<ServicoPrestador>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.Where(s => s.PrestadorId == prestadorId && s.Ativo).OrderBy(s => s.Titulo).AsEnumerable());

    public Task<IEnumerable<ServicoPrestador>> BuscarAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.Where(s => s.Ativo && (!categoria.HasValue || s.Categoria == categoria) && (!precoMaximo.HasValue || s.PrecoBase <= precoMaximo)).AsEnumerable());

    public Task<PaginacaoResultado<ServicoBase>> BuscarTodosPaginadoAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, TipoAnuncio? tipoAnuncio, Guid? usuarioId, int pagina, int tamanhoPagina, CancellationToken cancellationToken = default)
    {
        var itens = Itens.Values
            .Cast<ServicoBase>()
            .Where(s => !tipoAnuncio.HasValue || s.TipoAnuncio == tipoAnuncio.Value)
            .ToList();
        return Task.FromResult(new PaginacaoResultado<ServicoBase>
        {
            Itens = itens,
            PaginaAtual = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalRegistros = itens.Count,
            TotalPaginas = itens.Count == 0 ? 0 : 1
        });
    }

    public Task<ServicoPrestador?> ObterComAvaliacoesAsync(Guid servicoPrestadorId, CancellationToken cancellationToken = default) =>
        ObterPorIdAsync(servicoPrestadorId, cancellationToken);
}

internal sealed class FakeServicoClienteRepository : FakeRepositoryBase<ServicoCliente>, IServicoClienteRepository
{
    public Task<IEnumerable<ServicoCliente>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.Where(s => s.ClienteId == clienteId && s.Ativo).OrderBy(s => s.Titulo).AsEnumerable());

    public Task<IEnumerable<ServicoCliente>> BuscarPedidosAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.Where(s => s.Ativo && (!categoria.HasValue || s.Categoria == categoria) && (!precoMaximo.HasValue || s.PrecoBase <= precoMaximo)).AsEnumerable());
}

internal sealed class FakeAgendamentoRepository : FakeRepositoryBase<Agendamento>, IAgendamentoRepository
{
    public List<ServicoBase> Servicos { get; } = [];
    public Endereco? EnderecoPrincipal { get; set; }

    public Task<IEnumerable<Agendamento>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.Where(a => a.ClienteId == clienteId).AsEnumerable());

    public Task<IEnumerable<Agendamento>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.Where(a => a.PrestadorId == prestadorId).AsEnumerable());

    public Task<IEnumerable<Agendamento>> ObterSolicitacoesPendentesPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.Where(a => a.PrestadorId == prestadorId && a.Status == StatusAgendamento.Solicitado).AsEnumerable());

    public Task<IEnumerable<Agendamento>> ObterPorStatusAsync(StatusAgendamento status, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.Where(a => a.Status == status).AsEnumerable());

    public Task<List<ServicoBase>> ObterServicosPorIdsAsync(IEnumerable<Guid> servicosIds, CancellationToken cancellationToken = default) =>
        Task.FromResult(Servicos.Where(s => servicosIds.Contains(s.Id)).ToList());

    public Task<Endereco?> ObterEnderecoPrincipalDoClienteAsync(Guid clienteId, CancellationToken cancellationToken = default) => Task.FromResult(EnderecoPrincipal);
}

internal sealed class FakePrestadorRepository : FakeRepositoryBase<Prestador>, IPrestadorRepository
{
    public List<Agendamento> HistoricoServicos { get; } = [];

    public Task<Prestador?> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.FirstOrDefault(p => p.UsuarioId == usuarioId));

    public Task<IEnumerable<Prestador>> BuscarAsync(CategoriaServico? categoria, string? cidade, DateTime? dataDisponivel, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.AsEnumerable());

    public Task<IEnumerable<Agendamento>> ObterHistoricoServicosAsync(Guid prestadorId, CancellationToken cancellationToken = default) =>
        Task.FromResult(HistoricoServicos.Where(a => a.PrestadorId == prestadorId).AsEnumerable());

    public Task<Prestador?> ObterComAvaliacoesAsync(Guid prestadorId, CancellationToken cancellationToken = default) => ObterPorIdAsync(prestadorId, cancellationToken);
}

internal sealed class FakePagamentoRepository : FakeRepositoryBase<Pagamento>, IPagamentoRepository
{
    public Task<Pagamento?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.FirstOrDefault(p => p.AgendamentoId == agendamentoId));
}

internal sealed class FakePagamentoGateway : IPagamentoGateway
{
    public PagamentoCheckoutRequestDto? UltimoCheckoutRequest { get; private set; }

    public PagamentoCheckoutResponseDto CheckoutResponse { get; set; } = new()
    {
        CheckoutExternoId = "pref-123",
        CheckoutUrl = "https://checkout.test/pref-123",
        StatusExterno = "pending",
        PayloadExterno = "{}"
    };

    public PagamentoStatusGatewayDto? StatusResponse { get; set; }

    public Task<PagamentoCheckoutResponseDto> CriarCheckoutPixAsync(PagamentoCheckoutRequestDto pagamento, CancellationToken cancellationToken = default)
    {
        UltimoCheckoutRequest = pagamento;
        return Task.FromResult(CheckoutResponse);
    }

    public Task<PagamentoStatusGatewayDto?> ObterStatusPagamentoAsync(string pagamentoExternoId, CancellationToken cancellationToken = default) =>
        Task.FromResult(StatusResponse);
}

internal sealed class FakeClienteRepository : FakeRepositoryBase<Cliente>, IClienteRepository
{
    public Dictionary<Guid, int> TotalServicosConcluidosPorCliente { get; } = [];

    public Task<Cliente?> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.FirstOrDefault(c => c.UsuarioId == usuarioId));

    public Task<IEnumerable<Agendamento>> ObterHistoricoAgendamentosAsync(Guid clienteId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Enumerable.Empty<Agendamento>());

    public Task<Cliente?> ObterComAvaliacoesAsync(Guid clienteId, CancellationToken cancellationToken = default) =>
        ObterPorIdAsync(clienteId, cancellationToken);

    public Task<int> ObterTotalServicosContratadosConcluidosAsync(Guid clienteId, CancellationToken cancellationToken = default) =>
        Task.FromResult(TotalServicosConcluidosPorCliente.GetValueOrDefault(clienteId));
}

internal sealed class FakeMensagemRepository : FakeRepositoryBase<Mensagem>, IMensagemRepository
{
    public Task<IEnumerable<Mensagem>> ObterConversaAsync(Guid conversaId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.Where(m => m.ConversaId == conversaId).OrderBy(m => m.DataEnvio).AsEnumerable());

    public Task<IEnumerable<Mensagem>> ObterPorAgendamentoAsync(Guid agendamentoId, DateTime desde, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.Where(m => m.AgendamentoId == agendamentoId && m.DataEnvio >= desde).OrderBy(m => m.DataEnvio).AsEnumerable());

    public Task<List<Mensagem>> ObterUltimasPorAgendamentoAsync(Guid agendamentoId, int quantidade, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.Where(m => m.AgendamentoId == agendamentoId).OrderByDescending(m => m.DataEnvio).Take(quantidade).ToList());

    public Task<IEnumerable<Mensagem>> ObterConversasPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Enumerable.Empty<Mensagem>());

    public Task<int> ObterNaoLidasAsync(Guid usuarioId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.Count(m => m.RemetenteId != usuarioId && !m.Lida));

    public Task<int> RemoverEnviadasAntesDeAsync(DateTime dataLimite, CancellationToken cancellationToken = default)
    {
        var ids = Itens.Values.Where(m => m.DataEnvio < dataLimite).Select(m => m.Id).ToList();
        foreach (var id in ids)
            Itens.Remove(id);

        return Task.FromResult(ids.Count);
    }
}

internal sealed class FakeAvaliacaoRepository : FakeRepositoryBase<Avaliacao>, IAvaliacaoRepository
{
    public Dictionary<Guid, Agendamento> AgendamentosElegiveis { get; } = [];

    public Task<Avaliacao?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.FirstOrDefault(a => a.AgendamentoId == agendamentoId));

    public Task<IEnumerable<Avaliacao>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.Where(a => a.PrestadorId == prestadorId).AsEnumerable());

    public Task<IEnumerable<Avaliacao>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.Where(a => a.ClienteId == clienteId).AsEnumerable());

    public Task<bool> PodeAvaliarAsync(Guid clienteId, Guid agendamentoId, CancellationToken cancellationToken = default) =>
        Task.FromResult(AgendamentosElegiveis.TryGetValue(agendamentoId, out var agendamento) && agendamento.ClienteId == clienteId);

    public Task<Agendamento?> ObterAgendamentoElegivelParaAvaliacaoAsync(Guid clienteId, Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        AgendamentosElegiveis.TryGetValue(agendamentoId, out var agendamento);
        if (agendamento?.ClienteId != clienteId)
            agendamento = null;
        return Task.FromResult(agendamento);
    }
}

internal sealed class FakeAvaliacaoClienteRepository : FakeRepositoryBase<AvaliacaoCliente>, IAvaliacaoClienteRepository
{
    public Dictionary<Guid, Agendamento> AgendamentosElegiveis { get; } = [];

    public Task<AvaliacaoCliente?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.FirstOrDefault(a => a.AgendamentoId == agendamentoId));

    public Task<IEnumerable<AvaliacaoCliente>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.Where(a => a.ClienteId == clienteId).AsEnumerable());

    public Task<IEnumerable<AvaliacaoCliente>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Itens.Values.Where(a => a.PrestadorId == prestadorId).AsEnumerable());

    public Task<bool> PodeAvaliarAsync(Guid prestadorId, Guid agendamentoId, CancellationToken cancellationToken = default) =>
        Task.FromResult(AgendamentosElegiveis.TryGetValue(agendamentoId, out var agendamento) && agendamento.PrestadorId == prestadorId);

    public Task<Agendamento?> ObterAgendamentoElegivelParaAvaliacaoAsync(Guid prestadorId, Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        AgendamentosElegiveis.TryGetValue(agendamentoId, out var agendamento);
        if (agendamento?.PrestadorId != prestadorId)
            agendamento = null;
        return Task.FromResult(agendamento);
    }
}
