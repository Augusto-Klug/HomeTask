using Microsoft.EntityFrameworkCore;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Entities;
using HomeTask.Domain.Enums;
using HomeTask.Infrastructure.Data;

namespace HomeTask.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de serviços oferecidos (RF03, RF07)
/// </summary>
public class ServicoService : IServicoService
{
    private readonly HomeTaskDbContext _context;

    public ServicoService(HomeTaskDbContext context)
    {
        _context = context;
    }

    public async Task<ServicoOferecido?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ServicosOferecidos
            .Include(s => s.Prestador)
            .ThenInclude(p => p.Usuario)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<ServicoOferecido> CriarAsync(ServicoOferecido servico, CancellationToken cancellationToken = default)
    {
        servico.Ativar(DateTime.UtcNow);

        _context.ServicosOferecidos.Add(servico);
        await _context.SaveChangesAsync(cancellationToken);

        return servico;
    }

    public async Task<ServicoOferecido> AtualizarAsync(ServicoOferecido servico, CancellationToken cancellationToken = default)
    {
        _context.ServicosOferecidos.Update(servico);
        await _context.SaveChangesAsync(cancellationToken);
        return servico;
    }

    public async Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var servico = await _context.ServicosOferecidos.FindAsync([id], cancellationToken);
        if (servico == null)
            return false;

        servico.Desativar();
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<ServicoOferecido>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        return await _context.ServicosOferecidos
            .Where(s => s.PrestadorId == prestadorId && s.Ativo)
            .OrderBy(s => s.Titulo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ServicoOferecido>> BuscarAsync(Guid? categoriaId, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken = default)
    {
        var query = _context.ServicosOferecidos
            .Include(s => s.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(s => s.Prestador)
                .ThenInclude(p => p.Avaliacoes)
            .Include(s => s.Categoria)
            .Where(s => s.Ativo && s.PrestadorId != null && s.Prestador!.Status == StatusPrestador.Ativo);

        if (categoriaId.HasValue)
            query = query.Where(s => s.CategoriaId == categoriaId.Value);

        if (!string.IsNullOrWhiteSpace(cidade))
            query = query.Where(s =>
                s.Prestador!.Usuario.Enderecos
                    .Any(e => e.Principal && e.Cidade.Nome.Contains(cidade)));

        if (precoMaximo.HasValue)
            query = query.Where(s => s.PrecoBase <= precoMaximo.Value);

        return await query
            .OrderByDescending(s => s.Prestador!.MediaAvaliacoes)
            .ThenBy(s => s.PrecoBase)
            .ToListAsync(cancellationToken);
    }
}
