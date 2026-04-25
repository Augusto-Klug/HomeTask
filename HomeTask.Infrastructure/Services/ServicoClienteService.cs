using Microsoft.EntityFrameworkCore;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Enums;
using HomeTask.Infrastructure.Data;
using HomeTask.Domain.Entidades;

namespace HomeTask.Infrastructure.Services;

public class ServicoClienteService : IServicoClienteService
{
    private readonly HomeTaskDbContext _context;

    public ServicoClienteService(HomeTaskDbContext context)
    {
        _context = context;
    }

    public async Task<ServicoCliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var servico = await _context.ServicosClientes
            .Include(s => s.Cliente)
                .ThenInclude(c => c.Usuario)
                    .ThenInclude(u => u.Endereco)
                        .ThenInclude(e => e.Cidade)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        // Fallback para forçar o carregamento caso o Include falhe silenciosamente no EF Core
        if (servico != null && servico.Cliente?.Usuario != null && servico.Cliente.Usuario.Endereco == null)
        {
            await _context.Entry(servico.Cliente.Usuario).Reference(u => u.Endereco).LoadAsync(cancellationToken);
            if (servico.Cliente.Usuario.Endereco != null)
            {
                await _context.Entry(servico.Cliente.Usuario.Endereco).Reference(e => e.Cidade).LoadAsync(cancellationToken);
            }
        }

        return servico;
    }

    public async Task<ServicoCliente> CriarAsync(ServicoCliente servico, CancellationToken cancellationToken = default)
    {
        _context.ServicosClientes.Add(servico);
        await _context.SaveChangesAsync(cancellationToken);
        return servico;
    }

    public async Task<ServicoCliente> AtualizarAsync(ServicoCliente servico, CancellationToken cancellationToken = default)
    {
        _context.ServicosClientes.Update(servico);
        await _context.SaveChangesAsync(cancellationToken);
        return servico;
    }

    public async Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var servico = await _context.ServicosClientes.FindAsync([id], cancellationToken);
        if (servico == null) return false;
        servico.Desativar();
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<ServicoCliente>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        return await _context.ServicosClientes
            .Where(s => s.ClienteId == clienteId && s.Ativo)
            .OrderBy(s => s.Titulo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ServicoCliente>> BuscarPedidosAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken = default)
    {
        var query = _context.ServicosClientes
            .Include(s => s.Cliente)
                .ThenInclude(c => c.Usuario)
                    .ThenInclude(u => u.Endereco)
                        .ThenInclude(e => e.Cidade)
            .Where(s => s.Ativo);

        if (categoria.HasValue)
            query = query.Where(s => s.Categoria == categoria.Value);

        if (!string.IsNullOrWhiteSpace(cidade))
            query = query.Where(s => s.Cliente.Usuario.Endereco != null && 
                                     s.Cliente.Usuario.Endereco.Cidade.Nome.Contains(cidade));

        if (precoMaximo.HasValue)
            query = query.Where(s => s.PrecoBase <= precoMaximo.Value);

        return await query
            .OrderByDescending(s => s.DataCriacao)
            .ToListAsync(cancellationToken);
    }
}
