using Microsoft.EntityFrameworkCore;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Entities;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;
using HomeTask.Infrastructure.Data;

namespace HomeTask.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de avaliações (RF06, NEG06, NEG07)
/// </summary>
public class AvaliacaoService : IAvaliacaoService
{
    private readonly HomeTaskDbContext _context;
    private readonly IPrestadorService _prestadorService;

    public AvaliacaoService(HomeTaskDbContext context, IPrestadorService prestadorService)
    {
        _context = context;
        _prestadorService = prestadorService;
    }

    public async Task<Avaliacao?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Avaliacoes
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Prestador)
            .Include(a => a.Agendamento)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Avaliacao?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        return await _context.Avaliacoes
            .Include(a => a.Cliente)
            .FirstOrDefaultAsync(a => a.AgendamentoId == agendamentoId, cancellationToken);
    }

    public async Task<Avaliacao> CriarAsync(Avaliacao avaliacao, CancellationToken cancellationToken = default)
    {
        // NEG06 - Somente clientes que concluíram um serviço podem avaliar
        var podeAvaliar = await PodeAvaliarAsync(avaliacao.ClienteId, avaliacao.AgendamentoId, cancellationToken);
        if (!podeAvaliar)
            throw new InvalidOperationException("Somente serviços concluídos podem ser avaliados");

        var avaliacaoExistente = await ObterPorAgendamentoAsync(avaliacao.AgendamentoId, cancellationToken);
        if (avaliacaoExistente != null)
            throw new InvalidOperationException("Este serviço já foi avaliado");

        avaliacao.DataAvaliacao = DateTime.UtcNow;
        avaliacao.Visivel = true; // NEG07 - Avaliações são públicas

        _context.Avaliacoes.Add(avaliacao);
        await _context.SaveChangesAsync(cancellationToken);

        await _prestadorService.AtualizarMediaAvaliacoesAsync(avaliacao.PrestadorId, cancellationToken);

        return avaliacao;
    }

    public async Task<IEnumerable<Avaliacao>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        return await _context.Avaliacoes
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Agendamento)
                .ThenInclude(ag => ag.AgendamentoServicos)
                    .ThenInclude(ag => ag.ServicoOferecido)
            .Where(a => a.PrestadorId == prestadorId && a.Visivel)
            .OrderByDescending(a => a.DataAvaliacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Avaliacao>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        return await _context.Avaliacoes
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.Agendamento)
                .ThenInclude(ag => ag.AgendamentoServicos)
                    .ThenInclude(ags => ags.ServicoOferecido)
            .Where(a => a.ClienteId == clienteId)
            .OrderByDescending(a => a.DataAvaliacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> PodeAvaliarAsync(Guid clienteId, Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        var agendamento = await _context.Agendamentos
            .FirstOrDefaultAsync(a => a.Id == agendamentoId && a.ClienteId == clienteId, cancellationToken);

        if (agendamento == null)
            return false;

        // NEG06 - Somente serviços concluídos podem ser avaliados
        return agendamento.Status == StatusAgendamento.Concluido;
    }
}
