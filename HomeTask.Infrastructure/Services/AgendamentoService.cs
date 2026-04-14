using Microsoft.EntityFrameworkCore;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Entities;
using HomeTask.Domain.Enums;
using HomeTask.Infrastructure.Data;

namespace HomeTask.Infrastructure.Services;

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

    public async Task<Agendamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.Endereco)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(s => s.ServicoOferecido)
            .Include(a => a.Pagamento)
            .Include(a => a.Avaliacao)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Agendamento> CriarAsync(Agendamento agendamento, CancellationToken cancellationToken = default)
    {
        agendamento.DefinirComoSolicitado(DateTime.UtcNow);

        _context.Agendamentos.Add(agendamento);
        await _context.SaveChangesAsync(cancellationToken);

        return agendamento;
    }

    public async Task<Agendamento> AceitarAsync(Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        var agendamento = await _context.Agendamentos.FindAsync([agendamentoId], cancellationToken);
        if (agendamento == null)
            throw new InvalidOperationException("Agendamento não encontrado");

        if (agendamento.Status != StatusAgendamento.Solicitado)
            throw new InvalidOperationException("Agendamento não pode ser aceito neste status");

        agendamento.Aceitar(DateTime.UtcNow);

        await _context.SaveChangesAsync(cancellationToken);
        return agendamento;
    }

    public async Task<Agendamento> RecusarAsync(Guid agendamentoId, string motivo, CancellationToken cancellationToken = default)
    {
        var agendamento = await _context.Agendamentos.FindAsync([agendamentoId], cancellationToken);
        if (agendamento == null)
            throw new InvalidOperationException("Agendamento não encontrado");

        if (agendamento.Status != StatusAgendamento.Solicitado)
            throw new InvalidOperationException("Agendamento não pode ser recusado neste status");

        agendamento.Recusar(motivo, DateTime.UtcNow);

        await _context.SaveChangesAsync(cancellationToken);
        return agendamento;
    }

    public async Task<Agendamento> IniciarAsync(Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        var agendamento = await _context.Agendamentos.FindAsync([agendamentoId], cancellationToken);
        if (agendamento == null)
            throw new InvalidOperationException("Agendamento não encontrado");

        if (agendamento.Status != StatusAgendamento.Aceito)
            throw new InvalidOperationException("Agendamento não pode ser iniciado neste status");

        agendamento.Iniciar();

        await _context.SaveChangesAsync(cancellationToken);
        return agendamento;
    }

    public async Task<Agendamento> ConcluirAsync(Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        var agendamento = await _context.Agendamentos.FindAsync([agendamentoId], cancellationToken);
        if (agendamento == null)
            throw new InvalidOperationException("Agendamento não encontrado");

        if (agendamento.Status != StatusAgendamento.EmAndamento)
            throw new InvalidOperationException("Agendamento não pode ser concluído neste status");

        agendamento.Concluir(DateTime.UtcNow);

        var prestador = await _context.Prestadores.FindAsync([agendamento.PrestadorId], cancellationToken);
        if (prestador != null)
        {
            prestador.IncrementarTotalServicosConcluidos();
        }

        await _context.SaveChangesAsync(cancellationToken);
        return agendamento;
    }

    public async Task<Agendamento> CancelarAsync(Guid agendamentoId, string motivo, CancellationToken cancellationToken = default)
    {
        var agendamento = await _context.Agendamentos.FindAsync([agendamentoId], cancellationToken);
        if (agendamento == null)
            throw new InvalidOperationException("Agendamento não encontrado");

        if (agendamento.Status == StatusAgendamento.Concluido ||
            agendamento.Status == StatusAgendamento.Cancelado)
            throw new InvalidOperationException("Agendamento não pode ser cancelado neste status");

        agendamento.Cancelar(motivo);

        await _context.SaveChangesAsync(cancellationToken);
        return agendamento;
    }

    public async Task<IEnumerable<Agendamento>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(s => s.ServicoOferecido)
            .Where(a => a.ClienteId == clienteId)
            .OrderByDescending(a => a.DataHoraAgendada)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(s => s.ServicoOferecido)
            .Where(a => a.PrestadorId == prestadorId)
            .OrderByDescending(a => a.DataHoraAgendada)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterPorStatusAsync(StatusAgendamento status, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Prestador)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(s => s.ServicoOferecido)
            .Where(a => a.Status == status)
            .OrderByDescending(a => a.DataSolicitacao)
            .ToListAsync(cancellationToken);
    }
}
