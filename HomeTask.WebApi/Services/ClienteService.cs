using Microsoft.EntityFrameworkCore;
using HomeTask.WebApi.Data;
using HomeTask.WebApi.Models.Entities;
using HomeTask.WebApi.Services.Interfaces;

namespace HomeTask.WebApi.Services;

/// <summary>
/// Implementação do serviço de clientes (RF11)
/// </summary>
public class ClienteService : IClienteService
{
    private readonly HomeTaskDbContext _context;

    public ClienteService(HomeTaskDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> ObterPorIdAsync(Guid id)
    {
        return await _context.Clientes
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Cliente?> ObterPorUsuarioIdAsync(Guid usuarioId)
    {
        return await _context.Clientes
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
    }

    public async Task<Cliente> CriarAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }

    public async Task<Cliente> AtualizarAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }

    public async Task<IEnumerable<Agendamento>> ObterHistoricoAgendamentosAsync(Guid clienteId)
    {
        return await _context.Agendamentos
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.ServicoOferecido)
            .Include(a => a.Avaliacao)
            .Include(a => a.Pagamento)
            .Where(a => a.ClienteId == clienteId)
            .OrderByDescending(a => a.DataHoraAgendada)
            .ToListAsync();
    }
}
