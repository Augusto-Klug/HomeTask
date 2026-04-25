using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;

namespace HomeTask.Application.Interfaces;

public interface IServicoPrestadorService
{
    Task<ServicoPrestador?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServicoPrestador> CriarAsync(ServicoPrestador servico, CancellationToken cancellationToken = default);
    Task<ServicoPrestador> AtualizarAsync(ServicoPrestador servico, CancellationToken cancellationToken = default);
    Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServicoPrestador>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServicoPrestador>> BuscarAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServicoBase>> BuscarTodosAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken = default);
}

public interface IServicoClienteService
{
    Task<ServicoCliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServicoCliente> CriarAsync(ServicoCliente servico, CancellationToken cancellationToken = default);
    Task<ServicoCliente> AtualizarAsync(ServicoCliente servico, CancellationToken cancellationToken = default);
    Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServicoCliente>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServicoCliente>> BuscarPedidosAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken = default);
}
