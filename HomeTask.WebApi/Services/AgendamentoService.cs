using Microsoft.EntityFrameworkCore;
using HomeTask.WebApi.Data;
using HomeTask.WebApi.Models.Entities;
using HomeTask.WebApi.Models.Enums;
using HomeTask.WebApi.Services.Interfaces;

namespace HomeTask.WebApi.Services;

/// <summary>
/// Implementação do serviço de agendamentos (RF04, RF10)
/// </summary>
public class AgendamentoService : IAgendamentoService
{
    private readonly HomeTaskDbContext _context;

    public AgendamentoService(HomeTaskDbContext context)
    {
        _context = context;
    }

    public async Task<Agendamento?> ObterPorIdAsync(int id)
    {
        return await _context.Agendamentos
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.ServicoOferecido)
            .Include(a => a.Pagamento)
            .Include(a => a.Avaliacao)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Agendamento> CriarAsync(Agendamento agendamento)
    {
        agendamento.Status = StatusAgendamento.Solicitado;
        agendamento.DataSolicitacao = DateTime.UtcNow;
        
        _context.Agendamentos.Add(agendamento);
        await _context.SaveChangesAsync();
        
        return agendamento;
    }

    public async Task<Agendamento> AceitarAsync(int agendamentoId)
    {
        var agendamento = await _context.Agendamentos.FindAsync(agendamentoId);
        if (agendamento == null)
            throw new InvalidOperationException("Agendamento não encontrado");
        
        if (agendamento.Status != StatusAgendamento.Solicitado)
            throw new InvalidOperationException("Agendamento não pode ser aceito neste status");
        
        agendamento.Status = StatusAgendamento.Aceito;
        agendamento.DataResposta = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return agendamento;
    }

    public async Task<Agendamento> RecusarAsync(int agendamentoId, string motivo)
    {
        var agendamento = await _context.Agendamentos.FindAsync(agendamentoId);
        if (agendamento == null)
            throw new InvalidOperationException("Agendamento não encontrado");
        
        if (agendamento.Status != StatusAgendamento.Solicitado)
            throw new InvalidOperationException("Agendamento não pode ser recusado neste status");
        
        agendamento.Status = StatusAgendamento.Recusado;
        agendamento.DataResposta = DateTime.UtcNow;
        agendamento.MotivoRecusa = motivo;
        
        await _context.SaveChangesAsync();
        return agendamento;
    }

    public async Task<Agendamento> IniciarAsync(int agendamentoId)
    {
        var agendamento = await _context.Agendamentos.FindAsync(agendamentoId);
        if (agendamento == null)
            throw new InvalidOperationException("Agendamento não encontrado");
        
        if (agendamento.Status != StatusAgendamento.Aceito)
            throw new InvalidOperationException("Agendamento não pode ser iniciado neste status");
        
        agendamento.Status = StatusAgendamento.EmAndamento;
        
        await _context.SaveChangesAsync();
        return agendamento;
    }

    public async Task<Agendamento> ConcluirAsync(int agendamentoId)
    {
        var agendamento = await _context.Agendamentos.FindAsync(agendamentoId);
        if (agendamento == null)
            throw new InvalidOperationException("Agendamento não encontrado");
        
        if (agendamento.Status != StatusAgendamento.EmAndamento)
            throw new InvalidOperationException("Agendamento não pode ser concluído neste status");
        
        agendamento.Status = StatusAgendamento.Concluido;
        agendamento.DataConclusao = DateTime.UtcNow;
        
        // Incrementar contador de serviços do prestador
        var prestador = await _context.Prestadores.FindAsync(agendamento.PrestadorId);
        if (prestador != null)
        {
            prestador.TotalServicosConcluidos++;
        }
        
        await _context.SaveChangesAsync();
        return agendamento;
    }

    public async Task<Agendamento> CancelarAsync(int agendamentoId, string motivo)
    {
        var agendamento = await _context.Agendamentos.FindAsync(agendamentoId);
        if (agendamento == null)
            throw new InvalidOperationException("Agendamento não encontrado");
        
        if (agendamento.Status == StatusAgendamento.Concluido || 
            agendamento.Status == StatusAgendamento.Cancelado)
            throw new InvalidOperationException("Agendamento não pode ser cancelado neste status");
        
        agendamento.Status = StatusAgendamento.Cancelado;
        agendamento.MotivoRecusa = motivo;
        
        await _context.SaveChangesAsync();
        return agendamento;
    }

    public async Task<IEnumerable<Agendamento>> ObterPorClienteAsync(int clienteId)
    {
        return await _context.Agendamentos
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.ServicoOferecido)
            .Where(a => a.ClienteId == clienteId)
            .OrderByDescending(a => a.DataHoraAgendada)
            .ToListAsync();
    }

    public async Task<IEnumerable<Agendamento>> ObterPorPrestadorAsync(int prestadorId)
    {
        return await _context.Agendamentos
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.ServicoOferecido)
            .Where(a => a.PrestadorId == prestadorId)
            .OrderByDescending(a => a.DataHoraAgendada)
            .ToListAsync();
    }

    public async Task<IEnumerable<Agendamento>> ObterPorStatusAsync(StatusAgendamento status)
    {
        return await _context.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Prestador)
            .Include(a => a.ServicoOferecido)
            .Where(a => a.Status == status)
            .OrderByDescending(a => a.DataSolicitacao)
            .ToListAsync();
    }
}
