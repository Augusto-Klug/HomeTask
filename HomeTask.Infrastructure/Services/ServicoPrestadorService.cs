using Microsoft.EntityFrameworkCore;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Contratos;
using HomeTask.Domain.Enums;
using HomeTask.Infrastructure.Data;
using HomeTask.Domain.Entidades;

namespace HomeTask.Infrastructure.Services;

public class ServicoPrestadorService : IServicoPrestadorService
{
    private readonly HomeTaskDbContext _context;

    public ServicoPrestadorService(HomeTaskDbContext context)
    {
        _context = context;
    }

    public async Task<ServicoPrestador?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var servico = await _context.ServicosPrestadores
            .Include(s => s.Prestador)
                .ThenInclude(p => p.Usuario)
                    .ThenInclude(u => u.Endereco)
                        .ThenInclude(e => e.Cidade)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        // Fallback para forçar o carregamento caso o Include falhe silenciosamente no EF Core
        if (servico != null && servico.Prestador?.Usuario != null && servico.Prestador.Usuario.Endereco == null)
        {
            await _context.Entry(servico.Prestador.Usuario).Reference(u => u.Endereco).LoadAsync(cancellationToken);
            if (servico.Prestador.Usuario.Endereco != null)
            {
                await _context.Entry(servico.Prestador.Usuario.Endereco).Reference(e => e.Cidade).LoadAsync(cancellationToken);
            }
        }

        return servico;
    }

    public async Task<ServicoPrestador> CriarAsync(ServicoPrestador servico, CancellationToken cancellationToken = default)
    {
        _context.ServicosPrestadores.Add(servico);
        await _context.SaveChangesAsync(cancellationToken);
        return servico;
    }

    public async Task<ServicoPrestador> AtualizarAsync(ServicoPrestador servico, CancellationToken cancellationToken = default)
    {
        _context.ServicosPrestadores.Update(servico);
        await _context.SaveChangesAsync(cancellationToken);
        return servico;
    }

    public async Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var servico = await _context.ServicosPrestadores.FindAsync([id], cancellationToken);
        if (servico == null) return false;
        servico.Desativar();
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<ServicoPrestador>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        return await _context.ServicosPrestadores
            .Where(s => s.PrestadorId == prestadorId && s.Ativo)
            .OrderBy(s => s.Titulo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ServicoPrestador>> BuscarAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken = default)
    {
        var query = _context.ServicosPrestadores
            .Include(s => s.Prestador)
                .ThenInclude(p => p.Usuario)
            .Where(s => s.Ativo && s.Prestador.Status == StatusPrestador.Ativo);

        if (categoria.HasValue)
            query = query.Where(s => s.Categoria == categoria.Value);

        if (!string.IsNullOrWhiteSpace(cidade))
            query = query.Where(s => s.Prestador.Usuario.Endereco != null && 
                                     s.Prestador.Usuario.Endereco.Cidade.Nome.Contains(cidade));

        if (precoMaximo.HasValue)
            query = query.Where(s => s.PrecoBase <= precoMaximo.Value);

        return await query
            .OrderByDescending(s => s.Prestador.MediaAvaliacoes)
            .ThenBy(s => s.PrecoBase)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginacaoResultado<ServicoBase>> BuscarTodosPaginadoAsync(
        CategoriaServico? categoria,
        string? cidade,
        decimal? precoMaximo,
        Guid? usuarioId,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken = default)
    {
        var paginaAtual = pagina < 1 ? 1 : pagina;
        var tamanhoPaginaNormalizado = tamanhoPagina is 10 or 30 or 50 ? tamanhoPagina : 30;
        var queryPrestadores = _context.ServicosPrestadores
            .AsNoTracking()
            .Include(s => s.Prestador)
                .ThenInclude(p => p.Usuario)
                    .ThenInclude(u => u.Endereco)
                        .ThenInclude(e => e.Cidade)
            .Where(s => s.Ativo)
            .AsQueryable();

        var queryClientes = _context.ServicosClientes
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
            queryPrestadores = queryPrestadores.Where(s =>
                s.Prestador.Usuario.Endereco != null &&
                s.Prestador.Usuario.Endereco.Cidade.Nome.Contains(cidade));

            queryClientes = queryClientes.Where(s =>
                s.Cliente.Usuario.Endereco != null &&
                s.Cliente.Usuario.Endereco.Cidade.Nome.Contains(cidade));
        }

        var servicosPrestadores = await queryPrestadores.ToListAsync(cancellationToken);
        var servicosClientes = await queryClientes.ToListAsync(cancellationToken);

        var servicosOrdenados = servicosPrestadores
            .Cast<ServicoBase>()
            .Concat(servicosClientes)
            .OrderByDescending(s => s.DataCriacao)
            .ToList();

        var totalRegistros = servicosOrdenados.Count;
        var totalPaginas = totalRegistros == 0
            ? 0
            : (int)Math.Ceiling(totalRegistros / (double)tamanhoPaginaNormalizado);

        var itens = servicosOrdenados
            .Skip((paginaAtual - 1) * tamanhoPaginaNormalizado)
            .Take(tamanhoPaginaNormalizado)
            .ToList();

        return new PaginacaoResultado<ServicoBase>
        {
            Itens = itens,
            PaginaAtual = paginaAtual,
            TamanhoPagina = tamanhoPaginaNormalizado,
            TotalRegistros = totalRegistros,
            TotalPaginas = totalPaginas
        };
    }
}
