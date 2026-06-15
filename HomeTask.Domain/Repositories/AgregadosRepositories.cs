using HomeTask.Domain.Common;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Repositories;

public interface IUsuarioRepository : IRepositoryBase<Usuario>
{
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExisteCpfAsync(string cpf, CancellationToken cancellationToken = default);
}

public interface IClienteRepository : IRepositoryBase<Cliente>
{
    Task<Cliente?> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ObterHistoricoAgendamentosAsync(Guid clienteId, CancellationToken cancellationToken = default);
}

public interface IPrestadorRepository : IRepositoryBase<Prestador>
{
    Task<Prestador?> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Prestador>> BuscarAsync(CategoriaServico? categoria, string? cidade, DateTime? dataDisponivel, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ObterHistoricoServicosAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task<Prestador?> ObterComAvaliacoesAsync(Guid prestadorId, CancellationToken cancellationToken = default);
}

public interface IAgendamentoRepository : IRepositoryBase<Agendamento>
{
    Task<IEnumerable<Agendamento>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ObterSolicitacoesPendentesPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ObterPorStatusAsync(StatusAgendamento status, CancellationToken cancellationToken = default);
    Task<List<ServicoBase>> ObterServicosPorIdsAsync(IEnumerable<Guid> servicosIds, CancellationToken cancellationToken = default);
    Task<Endereco?> ObterEnderecoPrincipalDoClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
}

public interface IAvaliacaoRepository : IRepositoryBase<Avaliacao>
{
    Task<Avaliacao?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Avaliacao>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Avaliacao>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<bool> PodeAvaliarAsync(Guid clienteId, Guid agendamentoId, CancellationToken cancellationToken = default);
    Task<Agendamento?> ObterAgendamentoElegivelParaAvaliacaoAsync(Guid clienteId, Guid agendamentoId, CancellationToken cancellationToken = default);
}

public interface IPagamentoRepository : IRepositoryBase<Pagamento>
{
    Task<Pagamento?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default);
}

public interface IMensagemRepository : IRepositoryBase<Mensagem>
{
    Task<IEnumerable<Mensagem>> ObterConversaAsync(Guid conversaId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Mensagem>> ObterConversasPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<int> ObterNaoLidasAsync(Guid usuarioId, CancellationToken cancellationToken = default);
}

public interface IConversaRepository : IRepositoryBase<Conversa>
{
    Task<IEnumerable<Conversa>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<Conversa?> ObterPorClientePrestadorAsync(Guid clienteId, Guid prestadorId, CancellationToken cancellationToken = default);
}

public interface IServicoPrestadorRepository : IRepositoryBase<ServicoPrestador>
{
    Task<IEnumerable<ServicoPrestador>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServicoPrestador>> BuscarAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken = default);
    Task<ServicoPrestador?> ObterComAvaliacoesAsync(Guid servicoPrestadorId, CancellationToken cancellationToken = default);
    Task<PaginacaoResultado<ServicoBase>> BuscarTodosPaginadoAsync(
        CategoriaServico? categoria,
        string? cidade,
        decimal? precoMaximo,
        Guid? usuarioId,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken = default);
}

public interface IServicoClienteRepository : IRepositoryBase<ServicoCliente>
{
    Task<IEnumerable<ServicoCliente>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServicoCliente>> BuscarPedidosAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken = default);
}

public interface ICertificacaoRepository : IRepositoryBase<Certificacao>
{
    Task<IEnumerable<Certificacao>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
}

public interface IPortfolioRepository : IRepositoryBase<Portfolio>
{
    Task<IEnumerable<Portfolio>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
}

public interface ICidadeRepository : IRepositoryBase<Cidade>
{
    Task<IEnumerable<Cidade>> ListarAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Cidade>> BuscarAsync(string termo, CancellationToken cancellationToken = default);
}
