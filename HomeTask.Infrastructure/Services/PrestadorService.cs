using Microsoft.EntityFrameworkCore;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Entities;
using HomeTask.Domain.Enums;
using HomeTask.Infrastructure.Data;

namespace HomeTask.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de prestadores (RF02, RF07, RF09, RF12)
/// </summary>
public class PrestadorService : IPrestadorService
{
    private readonly HomeTaskDbContext _context;

    public PrestadorService(HomeTaskDbContext context)
    {
        _context = context;
    }

    public async Task<Prestador?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Prestadores
            .Include(p => p.Usuario)
            .Include(p => p.ServicosOferecidos)
            .Include(p => p.Certificacoes)
            .Include(p => p.Portfolios)
            .Include(p => p.Disponibilidades)
            .Include(p => p.Avaliacoes)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Prestador?> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return await _context.Prestadores
            .Include(p => p.Usuario)
            .Include(p => p.ServicosOferecidos)
            .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId, cancellationToken);
    }

    public async Task<Prestador> CriarAsync(Prestador prestador, CancellationToken cancellationToken = default)
    {
        prestador.DefinirStatus(StatusPrestador.EmAnalise);
        _context.Prestadores.Add(prestador);
        await _context.SaveChangesAsync(cancellationToken);
        return prestador;
    }

    public async Task<Prestador> AtualizarAsync(Prestador prestador, CancellationToken cancellationToken = default)
    {
        _context.Prestadores.Update(prestador);
        await _context.SaveChangesAsync(cancellationToken);
        return prestador;
    }

    public async Task<IEnumerable<Prestador>> BuscarAsync(CategoriaServico? categoria, string? cidade, DateTime? dataDisponivel, CancellationToken cancellationToken = default)
    {
        var query = _context.Prestadores
            .Include(p => p.Usuario)
                .ThenInclude(u => u.Endereco)
                    .ThenInclude(e => e.Cidade)
            .Include(p => p.ServicosOferecidos)
            .Include(p => p.Avaliacoes)
            .Where(p => p.Status == StatusPrestador.Ativo);

        if (categoria.HasValue)
        {
            query = query.Where(p => p.ServicosOferecidos
                .Any(s => s.Categoria == categoria.Value && s.Ativo));
        }

        if (!string.IsNullOrWhiteSpace(cidade))
        {
            query = query.Where(p => p.Usuario.Endereco != null && 
                                     p.Usuario.Endereco.Cidade.Nome.Contains(cidade));
        }

        if (dataDisponivel.HasValue)
        {
            var diaSemana = (int)dataDisponivel.Value.DayOfWeek;
            var hora = dataDisponivel.Value.TimeOfDay;

            query = query.Where(p => p.Disponibilidades.Any(d =>
                d.DiaSemana == diaSemana &&
                d.HoraInicio <= hora &&
                d.HoraFim >= hora &&
                d.Ativo));
        }

        return await query
            .OrderByDescending(p => p.MediaAvaliacoes)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterHistoricoServicosAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(s => s.ServicoBase)
            .Include(a => a.Avaliacao)
            .Where(a => a.PrestadorId == prestadorId)
            .OrderByDescending(a => a.DataHoraAgendada)
            .ToListAsync(cancellationToken);
    }

    public async Task AtualizarMediaAvaliacoesAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        var prestador = await _context.Prestadores
            .Include(p => p.Avaliacoes)
            .FirstOrDefaultAsync(p => p.Id == prestadorId, cancellationToken);

        if (prestador != null && prestador.Avaliacoes.Count != 0)
        {
            prestador.AtualizarMetricasAvaliacao(
                (decimal)prestador.Avaliacoes.Average(a => a.Nota),
                prestador.Avaliacoes.Count);

            // NEG08 - Suspensão automática para avaliações negativas recorrentes
            var avaliacoesRecentes = prestador.Avaliacoes
                .OrderByDescending(a => a.DataAvaliacao)
                .Take(5)
                .ToList();

            if (avaliacoesRecentes.Count >= 5 && avaliacoesRecentes.Average(a => a.Nota) < 2)
            {
                prestador.DefinirStatus(StatusPrestador.Suspenso);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task AtualizarStatusAsync(Guid prestadorId, StatusPrestador status, CancellationToken cancellationToken = default)
    {
        var prestador = await _context.Prestadores.FindAsync([prestadorId], cancellationToken);
        if (prestador != null)
        {
            prestador.DefinirStatus(status);
            if (status == StatusPrestador.Ativo)
            {
                prestador.DefinirDataVerificacao(DateTime.UtcNow);
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
