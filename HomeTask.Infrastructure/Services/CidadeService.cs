using Microsoft.EntityFrameworkCore;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Entidades;
using HomeTask.Infrastructure.Data;

namespace HomeTask.Infrastructure.Services;

public class CidadeService : ICidadeService
{
    private readonly HomeTaskDbContext _context;

    public CidadeService(HomeTaskDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cidade>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Cidades
            .OrderBy(c => c.Estado)
            .ThenBy(c => c.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Cidade>> BuscarAsync(string termo, CancellationToken cancellationToken = default)
    {
        return await _context.Cidades
            .Where(c => c.Nome.Contains(termo) || c.Estado.Contains(termo))
            .OrderBy(c => c.Estado)
            .ThenBy(c => c.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<Cidade?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Cidades
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
