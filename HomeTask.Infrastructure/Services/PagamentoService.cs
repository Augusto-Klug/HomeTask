using Microsoft.EntityFrameworkCore;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;
using HomeTask.Infrastructure.Data;

namespace HomeTask.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de pagamentos (RF05, NEG04)
/// </summary>
public class PagamentoService : IPagamentoService
{
    private readonly HomeTaskDbContext _context;

    public PagamentoService(HomeTaskDbContext context)
    {
        _context = context;
    }

    public async Task<Pagamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Pagamentos
            .Include(p => p.Agendamento)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Pagamento?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        return await _context.Pagamentos
            .Include(p => p.Agendamento)
            .FirstOrDefaultAsync(p => p.AgendamentoId == agendamentoId, cancellationToken);
    }

    public async Task<Pagamento> CriarAsync(Pagamento pagamento, CancellationToken cancellationToken = default)
    {
        pagamento.DefinirComoPendente(DateTime.UtcNow);

        _context.Pagamentos.Add(pagamento);
        await _context.SaveChangesAsync(cancellationToken);

        return pagamento;
    }

    public async Task<Pagamento> ProcessarAsync(Guid pagamentoId, CancellationToken cancellationToken = default)
    {
        var pagamento = await _context.Pagamentos.FindAsync([pagamentoId], cancellationToken);
        if (pagamento == null)
            throw new InvalidOperationException("Pagamento não encontrado");

        if (pagamento.Status != StatusPagamento.Pendente)
            throw new InvalidOperationException("Pagamento não pode ser processado neste status");

        pagamento.Processar(DateTime.UtcNow);

        // TODO: Integração com gateway de pagamento

        await _context.SaveChangesAsync(cancellationToken);
        return pagamento;
    }

    public async Task<Pagamento> ConfirmarAsync(Guid pagamentoId, string transacaoId, CancellationToken cancellationToken = default)
    {
        var pagamento = await _context.Pagamentos.FindAsync([pagamentoId], cancellationToken);
        if (pagamento == null)
            throw new InvalidOperationException("Pagamento não encontrado");

        if (pagamento.Status != StatusPagamento.Processando)
            throw new InvalidOperationException("Pagamento não pode ser confirmado neste status");

        pagamento.Aprovar(transacaoId, DateTime.UtcNow);

        await _context.SaveChangesAsync(cancellationToken);
        return pagamento;
    }

    public async Task<Pagamento> RecusarAsync(Guid pagamentoId, string motivo, CancellationToken cancellationToken = default)
    {
        var pagamento = await _context.Pagamentos.FindAsync([pagamentoId], cancellationToken);
        if (pagamento == null)
            throw new InvalidOperationException("Pagamento não encontrado");

        if (pagamento.Status != StatusPagamento.Processando)
            throw new InvalidOperationException("Pagamento não pode ser recusado neste status");

        pagamento.Recusar(motivo);

        await _context.SaveChangesAsync(cancellationToken);
        return pagamento;
    }

    public async Task<Pagamento> EstornarAsync(Guid pagamentoId, CancellationToken cancellationToken = default)
    {
        var pagamento = await _context.Pagamentos.FindAsync([pagamentoId], cancellationToken);
        if (pagamento == null)
            throw new InvalidOperationException("Pagamento não encontrado");

        if (pagamento.Status != StatusPagamento.Aprovado)
            throw new InvalidOperationException("Pagamento não pode ser estornado neste status");

        pagamento.Estornar();

        // TODO: Integração com gateway de pagamento para estorno

        await _context.SaveChangesAsync(cancellationToken);
        return pagamento;
    }
}
