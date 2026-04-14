using Microsoft.EntityFrameworkCore;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Entidades;
using HomeTask.Infrastructure.Data;

namespace HomeTask.Infrastructure.Services;

public class ConversaService : IConversaService
{
    private readonly HomeTaskDbContext _context;

    public ConversaService(HomeTaskDbContext context)
    {
        _context = context;
    }

    public async Task<Conversa?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Conversas
            .Include(c => c.Cliente)
                .ThenInclude(cl => cl.Usuario)
            .Include(c => c.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(c => c.Mensagens.OrderBy(m => m.DataEnvio))
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Conversa>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return await _context.Conversas
            .Include(c => c.Cliente)
                .ThenInclude(cl => cl.Usuario)
            .Include(c => c.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(c => c.Mensagens.OrderByDescending(m => m.DataEnvio).Take(1))
            .Where(c => c.Cliente.UsuarioId == usuarioId || c.Prestador.UsuarioId == usuarioId)
            .OrderByDescending(c => c.Mensagens.Max(m => (DateTime?)m.DataEnvio) ?? c.DataCriacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<Conversa> ObterOuCriarAsync(Guid clienteId, Guid prestadorId, CancellationToken cancellationToken = default)
    {
        // verifica se já existe conversa entre os dois
        var conversa = await _context.Conversas
            .FirstOrDefaultAsync(c =>
                c.ClienteId == clienteId &&
                c.PrestadorId == prestadorId,
                cancellationToken);

        if (conversa != null)
            return conversa;

        // cria nova conversa
        conversa = new Conversa
        {
            ClienteId = clienteId,
            PrestadorId = prestadorId,
            DataCriacao = DateTime.UtcNow
        };

        _context.Conversas.Add(conversa);
        await _context.SaveChangesAsync(cancellationToken);

        return conversa;
    }
}
