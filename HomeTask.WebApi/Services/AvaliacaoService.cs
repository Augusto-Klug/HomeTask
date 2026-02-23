using Microsoft.EntityFrameworkCore;
using HomeTask.WebApi.Data;
using HomeTask.WebApi.Models.Entities;
using HomeTask.WebApi.Models.Enums;
using HomeTask.WebApi.Services.Interfaces;

namespace HomeTask.WebApi.Services;

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

    public async Task<Avaliacao?> ObterPorIdAsync(int id)
    {
        return await _context.Avaliacoes
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Prestador)
            .Include(a => a.Agendamento)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Avaliacao?> ObterPorAgendamentoAsync(int agendamentoId)
    {
        return await _context.Avaliacoes
            .Include(a => a.Cliente)
            .FirstOrDefaultAsync(a => a.AgendamentoId == agendamentoId);
    }

    public async Task<Avaliacao> CriarAsync(Avaliacao avaliacao)
    {
        // NEG06 - Somente clientes que concluíram um serviço podem avaliar
        var podeAvaliar = await PodeAvaliarAsync(avaliacao.ClienteId, avaliacao.AgendamentoId);
        if (!podeAvaliar)
            throw new InvalidOperationException("Somente serviços concluídos podem ser avaliados");

        // Verificar se já existe avaliação para este agendamento
        var avaliacaoExistente = await ObterPorAgendamentoAsync(avaliacao.AgendamentoId);
        if (avaliacaoExistente != null)
            throw new InvalidOperationException("Este serviço já foi avaliado");

        avaliacao.DataAvaliacao = DateTime.UtcNow;
        avaliacao.Visivel = true; // NEG07 - Avaliações são públicas

        _context.Avaliacoes.Add(avaliacao);
        await _context.SaveChangesAsync();

        // Atualizar média de avaliações do prestador
        await _prestadorService.AtualizarMediaAvaliacoesAsync(avaliacao.PrestadorId);

        return avaliacao;
    }

    public async Task<IEnumerable<Avaliacao>> ObterPorPrestadorAsync(int prestadorId)
    {
        return await _context.Avaliacoes
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Agendamento)
                .ThenInclude(ag => ag.ServicoOferecido)
            .Where(a => a.PrestadorId == prestadorId && a.Visivel)
            .OrderByDescending(a => a.DataAvaliacao)
            .ToListAsync();
    }

    public async Task<IEnumerable<Avaliacao>> ObterPorClienteAsync(int clienteId)
    {
        return await _context.Avaliacoes
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.Agendamento)
                .ThenInclude(ag => ag.ServicoOferecido)
            .Where(a => a.ClienteId == clienteId)
            .OrderByDescending(a => a.DataAvaliacao)
            .ToListAsync();
    }

    public async Task<bool> PodeAvaliarAsync(int clienteId, int agendamentoId)
    {
        var agendamento = await _context.Agendamentos
            .FirstOrDefaultAsync(a => a.Id == agendamentoId && a.ClienteId == clienteId);

        if (agendamento == null)
            return false;

        // NEG06 - Somente serviços concluídos podem ser avaliados
        return agendamento.Status == StatusAgendamento.Concluido;
    }
}
