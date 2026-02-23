using Microsoft.EntityFrameworkCore;
using HomeTask.WebApi.Data;
using HomeTask.WebApi.Models.Entities;
using HomeTask.WebApi.Models.Enums;
using HomeTask.WebApi.Services.Interfaces;

namespace HomeTask.WebApi.Services;

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

    public async Task<ServicoOferecido?> ObterPorIdAsync(Guid id)
    {
        return await _context.ServicosOferecidos
            .Include(s => s.Prestador)
                .ThenInclude(p => p.Usuario)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<ServicoOferecido> CriarAsync(ServicoOferecido servico)
    {
        servico.DataCriacao = DateTime.UtcNow;
        servico.Ativo = true;
        
        _context.ServicosOferecidos.Add(servico);
        await _context.SaveChangesAsync();
        
        return servico;
    }

    public async Task<ServicoOferecido> AtualizarAsync(ServicoOferecido servico)
    {
        _context.ServicosOferecidos.Update(servico);
        await _context.SaveChangesAsync();
        return servico;
    }

    public async Task<bool> RemoverAsync(Guid id)
    {
        var servico = await _context.ServicosOferecidos.FindAsync(id);
        if (servico == null)
            return false;

        servico.Ativo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<ServicoOferecido>> ObterPorPrestadorAsync(Guid prestadorId)
    {
        return await _context.ServicosOferecidos
            .Where(s => s.PrestadorId == prestadorId && s.Ativo)
            .OrderBy(s => s.Categoria)
            .ToListAsync();
    }

    public async Task<IEnumerable<ServicoOferecido>> BuscarAsync(CategoriaServico? categoria, string? cidade, decimal? precoMaximo)
    {
        var query = _context.ServicosOferecidos
            .Include(s => s.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(s => s.Prestador)
                .ThenInclude(p => p.Avaliacoes)
            .Where(s => s.Ativo && s.Prestador.Status == StatusPrestador.Ativo);

        if (categoria.HasValue)
        {
            query = query.Where(s => s.Categoria == categoria.Value);
        }

        if (!string.IsNullOrWhiteSpace(cidade))
        {
            query = query.Where(s => s.Prestador.Cidade != null && s.Prestador.Cidade.Contains(cidade));
        }

        if (precoMaximo.HasValue)
        {
            query = query.Where(s => s.PrecoBase <= precoMaximo.Value);
        }

        return await query
            .OrderByDescending(s => s.Prestador.MediaAvaliacoes)
            .ThenBy(s => s.PrecoBase)
            .ToListAsync();
    }
}
