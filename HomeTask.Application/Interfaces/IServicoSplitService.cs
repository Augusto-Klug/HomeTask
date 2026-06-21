using HomeTask.Application.Dtos;
using HomeTask.Domain.Enums;

namespace HomeTask.Application.Interfaces;

public interface IServicoPrestadorService
{
    Task<ServicoPrestadorDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServicoPrestadorDto> CriarAsync(ServicoPrestadorDto servico, CancellationToken cancellationToken = default);
    Task<ServicoPrestadorDto> AtualizarAsync(ServicoPrestadorDto servico, CancellationToken cancellationToken = default);
    Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServicoPrestadorDto>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServicoPrestadorDto>> BuscarAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken = default);
    Task AtualizarMediaAvaliacoesAsync(Guid servicoPrestadorId, CancellationToken cancellationToken = default);
    Task<ServicoBuscaPaginadaDto> BuscarTodosPaginadoAsync(
        CategoriaServico? categoria,
        string? cidade,
        decimal? precoMaximo,
        TipoAnuncio? tipoAnuncio,
        Guid? usuarioId,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken = default);
}

public interface IServicoClienteService
{
    Task<ServicoClienteDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServicoClienteDto> CriarAsync(ServicoClienteDto servico, CancellationToken cancellationToken = default);
    Task<ServicoClienteDto> AtualizarAsync(ServicoClienteDto servico, CancellationToken cancellationToken = default);
    Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServicoClienteDto>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServicoClienteDto>> BuscarPedidosAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken = default);
}
