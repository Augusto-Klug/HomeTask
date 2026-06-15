using HomeTask.Domain.Common;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;
using HomeTask.Domain.Repositories;
using HomeTask.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeTask.Infrastructure.Repositories;

public class ServicoPrestadorRepository : RepositoryBase<ServicoPrestador>, IServicoPrestadorRepository
{
    public ServicoPrestadorRepository(HomeTaskDbContext context) : base(context)
    {
    }

    public override async Task<ServicoPrestador?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var servico = await Context.ServicosPrestadores
            .Include(s => s.Prestador)
                .ThenInclude(p => p.Usuario)
                    .ThenInclude(u => u.Endereco)
                        .ThenInclude(e => e.Cidade)
            .Include(s => s.Avaliacoes)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (servico != null && servico.Prestador?.Usuario != null && servico.Prestador.Usuario.Endereco == null)
        {
            await Context.Entry(servico.Prestador.Usuario).Reference(u => u.Endereco).LoadAsync(cancellationToken);
            if (servico.Prestador.Usuario.Endereco != null)
                await Context.Entry(servico.Prestador.Usuario.Endereco).Reference(e => e.Cidade).LoadAsync(cancellationToken);
        }

        return servico;
    }

    public async Task<IEnumerable<ServicoPrestador>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default) =>
        await Context.ServicosPrestadores
            .Where(s => s.PrestadorId == prestadorId && s.Ativo)
            .OrderBy(s => s.Titulo)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<ServicoPrestador>> BuscarAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken = default)
    {
        var query = Context.ServicosPrestadores
            .Include(s => s.Prestador)
                .ThenInclude(p => p.Usuario)
                    .ThenInclude(u => u.Endereco)
                        .ThenInclude(e => e.Cidade)
            .Where(s => s.Ativo && s.Prestador.Status == StatusPrestador.Ativo);

        if (categoria.HasValue)
            query = query.Where(s => s.Categoria == categoria.Value);

        if (!string.IsNullOrWhiteSpace(cidade))
            query = query.Where(s => s.Prestador.Usuario.Endereco != null && s.Prestador.Usuario.Endereco.Cidade.Nome.Contains(cidade));

        if (precoMaximo.HasValue)
            query = query.Where(s => s.PrecoBase <= precoMaximo.Value);

        return await query
            .OrderByDescending(s => (double)s.Prestador.MediaAvaliacoes)
            .ThenBy(s => (double)s.PrecoBase)
            .ToListAsync(cancellationToken);
    }

    public Task<ServicoPrestador?> ObterComAvaliacoesAsync(Guid servicoPrestadorId, CancellationToken cancellationToken = default) =>
        Context.ServicosPrestadores
            .Include(s => s.Avaliacoes)
            .FirstOrDefaultAsync(s => s.Id == servicoPrestadorId, cancellationToken);

    public async Task<PaginacaoResultado<ServicoBase>> BuscarTodosPaginadoAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, Guid? usuarioId, int pagina, int tamanhoPagina, CancellationToken cancellationToken = default)
    {
        var paginaAtual = pagina < 1 ? 1 : pagina;
        var tamanhoPaginaNormalizado = tamanhoPagina is 10 or 30 or 50 ? tamanhoPagina : 30;

        var queryPrestadores = Context.ServicosPrestadores
            .AsNoTracking()
            .Include(s => s.Prestador)
                .ThenInclude(p => p.Usuario)
                    .ThenInclude(u => u.Endereco)
                        .ThenInclude(e => e.Cidade)
            .Where(s => s.Ativo)
            .AsQueryable();

        var queryClientes = Context.ServicosClientes
            .AsNoTracking()
            .Include(s => s.Cliente)
                .ThenInclude(c => c.Usuario)
                    .ThenInclude(u => u.Endereco)
                        .ThenInclude(e => e.Cidade)
            .Where(s => s.Ativo)
            .AsQueryable();

        if (categoria.HasValue)
        {
            queryPrestadores = queryPrestadores.Where(s => s.Categoria == categoria.Value);
            queryClientes = queryClientes.Where(s => s.Categoria == categoria.Value);
        }

        if (precoMaximo.HasValue)
        {
            queryPrestadores = queryPrestadores.Where(s => s.PrecoBase <= precoMaximo.Value);
            queryClientes = queryClientes.Where(s => s.PrecoBase <= precoMaximo.Value);
        }

        if (!string.IsNullOrWhiteSpace(cidade))
        {
            queryPrestadores = queryPrestadores.Where(s => s.Prestador.Usuario.Endereco != null && s.Prestador.Usuario.Endereco.Cidade.Nome.Contains(cidade));
            queryClientes = queryClientes.Where(s => s.Cliente.Usuario.Endereco != null && s.Cliente.Usuario.Endereco.Cidade.Nome.Contains(cidade));
        }

        if (usuarioId.HasValue)
        {
            queryPrestadores = queryPrestadores.Where(s => s.Prestador.UsuarioId != usuarioId.Value);
            queryClientes = queryClientes.Where(s => s.Cliente.UsuarioId != usuarioId.Value);
        }

        var servicosPrestadores = await queryPrestadores.ToListAsync(cancellationToken);
        var servicosClientes = await queryClientes.ToListAsync(cancellationToken);

        var servicosOrdenados = servicosPrestadores
            .Cast<ServicoBase>()
            .Concat(servicosClientes)
            .OrderByDescending(s => s.DataCriacao)
            .ToList();

        var totalRegistros = servicosOrdenados.Count;
        var totalPaginas = totalRegistros == 0 ? 0 : (int)Math.Ceiling(totalRegistros / (double)tamanhoPaginaNormalizado);

        return new PaginacaoResultado<ServicoBase>
        {
            Itens = servicosOrdenados.Skip((paginaAtual - 1) * tamanhoPaginaNormalizado).Take(tamanhoPaginaNormalizado).ToList(),
            PaginaAtual = paginaAtual,
            TamanhoPagina = tamanhoPaginaNormalizado,
            TotalRegistros = totalRegistros,
            TotalPaginas = totalPaginas
        };
    }
}

public class ServicoClienteRepository : RepositoryBase<ServicoCliente>, IServicoClienteRepository
{
    public ServicoClienteRepository(HomeTaskDbContext context) : base(context)
    {
    }

    public override async Task<ServicoCliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var servico = await Context.ServicosClientes
            .Include(s => s.Cliente)
                .ThenInclude(c => c.Usuario)
                    .ThenInclude(u => u.Endereco)
                        .ThenInclude(e => e.Cidade)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (servico != null && servico.Cliente?.Usuario != null && servico.Cliente.Usuario.Endereco == null)
        {
            await Context.Entry(servico.Cliente.Usuario).Reference(u => u.Endereco).LoadAsync(cancellationToken);
            if (servico.Cliente.Usuario.Endereco != null)
                await Context.Entry(servico.Cliente.Usuario.Endereco).Reference(e => e.Cidade).LoadAsync(cancellationToken);
        }

        return servico;
    }

    public async Task<IEnumerable<ServicoCliente>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default) =>
        await Context.ServicosClientes
            .Where(s => s.ClienteId == clienteId && s.Ativo)
            .OrderBy(s => s.Titulo)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<ServicoCliente>> BuscarPedidosAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken = default)
    {
        var query = Context.ServicosClientes
            .Include(s => s.Cliente)
                .ThenInclude(c => c.Usuario)
                    .ThenInclude(u => u.Endereco)
                        .ThenInclude(e => e.Cidade)
            .Where(s => s.Ativo);

        if (categoria.HasValue)
            query = query.Where(s => s.Categoria == categoria.Value);

        if (!string.IsNullOrWhiteSpace(cidade))
            query = query.Where(s => s.Cliente.Usuario.Endereco != null && s.Cliente.Usuario.Endereco.Cidade.Nome.Contains(cidade));

        if (precoMaximo.HasValue)
            query = query.Where(s => s.PrecoBase <= precoMaximo.Value);

        return await query.OrderByDescending(s => s.DataCriacao).ToListAsync(cancellationToken);
    }
}

public class PortfolioRepository : RepositoryBase<Portfolio>, IPortfolioRepository
{
    public PortfolioRepository(HomeTaskDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Portfolio>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default) =>
        await Context.Portfolios
            .AsNoTracking()
            .Where(p => p.PrestadorId == prestadorId)
            .OrderBy(p => p.Ordem)
            .ToListAsync(cancellationToken);
}

public class CertificacaoRepository : RepositoryBase<Certificacao>, ICertificacaoRepository
{
    public CertificacaoRepository(HomeTaskDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Certificacao>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default) =>
        await Context.Certificacoes
            .AsNoTracking()
            .Where(c => c.PrestadorId == prestadorId)
            .OrderByDescending(c => c.DataCadastro)
            .ToListAsync(cancellationToken);
}
