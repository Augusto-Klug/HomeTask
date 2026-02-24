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

    public async Task<Mensagem?> ObterPorIdAsync(Guid id)
    {
        return await _context.Mensagens
            .Include(m => m.Remetente)
            .Include(m => m.Destinatario)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Mensagem> EnviarAsync(Mensagem mensagem)
    {
        mensagem.DataEnvio = DateTime.UtcNow;
        mensagem.Lida = false;

        _context.Mensagens.Add(mensagem);
        await _context.SaveChangesAsync();

        return mensagem;
    }

    public async Task<IEnumerable<Mensagem>> ObterConversaAsync(Guid usuarioId1, Guid usuarioId2)
    {
        return await _context.Mensagens
            .Include(m => m.Remetente)
            .Include(m => m.Destinatario)
            .Where(m =>
                (m.RemetenteId == usuarioId1 && m.DestinatarioId == usuarioId2) ||
                (m.RemetenteId == usuarioId2 && m.DestinatarioId == usuarioId1))
            .OrderBy(m => m.DataEnvio)
            .ToListAsync();
    }

    public async Task<IEnumerable<Mensagem>> ObterConversasPorUsuarioAsync(Guid usuarioId)
    {
        var mensagens = await _context.Mensagens
            .Include(m => m.Remetente)
            .Include(m => m.Destinatario)
            .Where(m => m.RemetenteId == usuarioId || m.DestinatarioId == usuarioId)
            .ToListAsync();

        return mensagens
            .GroupBy(m => m.RemetenteId == usuarioId ? m.DestinatarioId : m.RemetenteId)
            .Select(g => g.OrderByDescending(m => m.DataEnvio).First())
            .OrderByDescending(m => m.DataEnvio);
    }

    public async Task MarcarComoLidaAsync(Guid mensagemId)
    {
        var mensagem = await _context.Mensagens.FindAsync(mensagemId);
        if (mensagem != null && !mensagem.Lida)
        {
            mensagem.Lida = true;
            mensagem.DataLeitura = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> ObterNaoLidasAsync(Guid usuarioId)
    {
        return await _context.Mensagens
            .CountAsync(m => m.DestinatarioId == usuarioId && !m.Lida);
    }
}
