using Microsoft.EntityFrameworkCore;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Entities;
using HomeTask.Infrastructure.Data;

namespace HomeTask.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de mensagens/chat (RF08, NEG09)
/// </summary>
public class MensagemService : IMensagemService
{
    private readonly HomeTaskDbContext _context;

    public MensagemService(HomeTaskDbContext context)
    {
        _context = context;
    }

    public async Task<Mensagem?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Mensagens
            .Include(m => m.Remetente)
            .Include(m => m.Conversa)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<Mensagem> EnviarAsync(Mensagem mensagem, CancellationToken cancellationToken = default)
    {
        mensagem.DataEnvio = DateTime.UtcNow;
        mensagem.Lida = false;

        _context.Mensagens.Add(mensagem);
        await _context.SaveChangesAsync(cancellationToken);

        return mensagem;
    }

    public async Task<IEnumerable<Mensagem>> ObterConversaAsync(Guid conversaId, CancellationToken cancellationToken = default)
    {
        return await _context.Mensagens
            .Include(m => m.Remetente)
            .Include(m => m.Conversa)
            .Where(m => m.ConversaId == conversaId)
            .OrderBy(m => m.DataEnvio)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Mensagem>> ObterConversasPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return await _context.Mensagens
            .Include(m => m.Remetente)
            .Include(m => m.Conversa)
            .Where(m => m.Conversa.ClienteId == usuarioId || m.Conversa.PrestadorId == usuarioId)
            .GroupBy(m => m.ConversaId)
            .Select(g => g.OrderByDescending(m => m.DataEnvio).First())
            .OrderByDescending(m => m.DataEnvio)
            .ToListAsync(cancellationToken);
    }

    public async Task MarcarComoLidaAsync(Guid mensagemId, CancellationToken cancellationToken = default)
    {
        var mensagem = await _context.Mensagens.FindAsync([mensagemId], cancellationToken);
        if (mensagem != null && !mensagem.Lida)
        {
            mensagem.Lida = true;
            mensagem.DataLeitura = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> ObterNaoLidasAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return await _context.Mensagens
            .Where(m => m.Conversa.ClienteId == usuarioId || m.Conversa.PrestadorId == usuarioId)
            .Where(m => m.RemetenteId != usuarioId && !m.Lida)
            .CountAsync(cancellationToken);
    }
}
