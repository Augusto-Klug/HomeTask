using HomeTask.Domain.Common;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;
using HomeTask.Domain.Repositories;
using HomeTask.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeTask.Infrastructure.Repositories;

public class AgendamentoRepository : RepositoryBase<Agendamento>, IAgendamentoRepository
{
    public AgendamentoRepository(HomeTaskDbContext context) : base(context)
    {
    }

    public override Task<Agendamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Context.Agendamentos
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.Endereco)
                .ThenInclude(e => e.Cidade)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(s => s.ServicoBase)
            .Include(a => a.Pagamento)
            .Include(a => a.Avaliacao)
            .Include(a => a.AvaliacaoCliente)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IEnumerable<Agendamento>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default) =>
        await Context.Agendamentos
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Endereco)
                .ThenInclude(e => e.Cidade)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(s => s.ServicoBase)
            .Include(a => a.Pagamento)
            .Include(a => a.Avaliacao)
            .Include(a => a.AvaliacaoCliente)
            .Where(a => a.ClienteId == clienteId)
            .OrderByDescending(a => a.DataHoraAgendada)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Agendamento>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default) =>
        await Context.Agendamentos
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.Endereco)
                .ThenInclude(e => e.Cidade)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(s => s.ServicoBase)
            .Include(a => a.Pagamento)
            .Include(a => a.Avaliacao)
            .Include(a => a.AvaliacaoCliente)
            .Where(a => a.PrestadorId == prestadorId)
            .OrderByDescending(a => a.DataHoraAgendada)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Agendamento>> ObterSolicitacoesPendentesPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default) =>
        await Context.Agendamentos
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.Endereco)
                .ThenInclude(e => e.Cidade)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(s => s.ServicoBase)
            .Include(a => a.Pagamento)
            .Include(a => a.Avaliacao)
            .Include(a => a.AvaliacaoCliente)
            .Where(a => a.PrestadorId == prestadorId && a.Status == StatusAgendamento.Solicitado)
            .OrderByDescending(a => a.DataSolicitacao)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Agendamento>> ObterPorStatusAsync(StatusAgendamento status, CancellationToken cancellationToken = default) =>
        await Context.Agendamentos
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.Endereco)
                .ThenInclude(e => e.Cidade)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(s => s.ServicoBase)
            .Include(a => a.Pagamento)
            .Include(a => a.Avaliacao)
            .Include(a => a.AvaliacaoCliente)
            .Where(a => a.Status == status)
            .OrderByDescending(a => a.DataSolicitacao)
            .ToListAsync(cancellationToken);

    public Task<List<ServicoBase>> ObterServicosPorIdsAsync(IEnumerable<Guid> servicosIds, CancellationToken cancellationToken = default) =>
        Context.Servicos.Where(s => servicosIds.Contains(s.Id)).ToListAsync(cancellationToken);

    public Task<Endereco?> ObterEnderecoPrincipalDoClienteAsync(Guid clienteId, CancellationToken cancellationToken = default) =>
        Context.Enderecos
            .FirstOrDefaultAsync(e => e.Usuario.Cliente != null && e.Usuario.Cliente.Id == clienteId, cancellationToken);
}

public class AvaliacaoRepository : RepositoryBase<Avaliacao>, IAvaliacaoRepository
{
    public AvaliacaoRepository(HomeTaskDbContext context) : base(context)
    {
    }

    public override Task<Avaliacao?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Context.Avaliacoes
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.ServicoPrestador)
            .Include(a => a.Agendamento)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<Avaliacao?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default) =>
        Context.Avaliacoes
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.ServicoPrestador)
            .FirstOrDefaultAsync(a => a.AgendamentoId == agendamentoId, cancellationToken);

    public async Task<IEnumerable<Avaliacao>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default) =>
        await Context.Avaliacoes
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.ServicoPrestador)
            .Include(a => a.Agendamento)
                .ThenInclude(ag => ag.AgendamentoServicos)
                    .ThenInclude(ags => ags.ServicoBase)
            .Where(a => a.PrestadorId == prestadorId && a.Visivel)
            .OrderByDescending(a => a.DataAvaliacao)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Avaliacao>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default) =>
        await Context.Avaliacoes
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.ServicoPrestador)
            .Include(a => a.Agendamento)
                .ThenInclude(ag => ag.AgendamentoServicos)
                    .ThenInclude(ags => ags.ServicoBase)
            .Where(a => a.ClienteId == clienteId)
            .OrderByDescending(a => a.DataAvaliacao)
            .ToListAsync(cancellationToken);

    public async Task<bool> PodeAvaliarAsync(Guid clienteId, Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        var agendamento = await Context.Agendamentos
            .FirstOrDefaultAsync(a => a.Id == agendamentoId && a.ClienteId == clienteId, cancellationToken);

        return agendamento?.Status == StatusAgendamento.Concluido;
    }

    public Task<Agendamento?> ObterAgendamentoElegivelParaAvaliacaoAsync(Guid clienteId, Guid agendamentoId, CancellationToken cancellationToken = default) =>
        Context.Agendamentos
            .Include(a => a.Pagamento)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(item => item.ServicoBase)
            .FirstOrDefaultAsync(
                a => a.Id == agendamentoId
                    && a.ClienteId == clienteId
                    && a.Status == StatusAgendamento.Concluido
                    && a.Pagamento != null
                    && a.Pagamento.Status == StatusPagamento.Aprovado,
                cancellationToken);
}

public class AvaliacaoClienteRepository : RepositoryBase<AvaliacaoCliente>, IAvaliacaoClienteRepository
{
    public AvaliacaoClienteRepository(HomeTaskDbContext context) : base(context)
    {
    }

    public override Task<AvaliacaoCliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Context.AvaliacoesClientes
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.Agendamento)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<AvaliacaoCliente?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default) =>
        Context.AvaliacoesClientes
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .FirstOrDefaultAsync(a => a.AgendamentoId == agendamentoId, cancellationToken);

    public async Task<IEnumerable<AvaliacaoCliente>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default) =>
        await Context.AvaliacoesClientes
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.Agendamento)
            .Where(a => a.ClienteId == clienteId && a.Visivel)
            .OrderByDescending(a => a.DataAvaliacao)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<AvaliacaoCliente>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default) =>
        await Context.AvaliacoesClientes
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Agendamento)
            .Where(a => a.PrestadorId == prestadorId)
            .OrderByDescending(a => a.DataAvaliacao)
            .ToListAsync(cancellationToken);

    public async Task<bool> PodeAvaliarAsync(Guid prestadorId, Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        var agendamento = await Context.Agendamentos
            .Include(a => a.Pagamento)
            .FirstOrDefaultAsync(a => a.Id == agendamentoId && a.PrestadorId == prestadorId, cancellationToken);

        return agendamento?.Status == StatusAgendamento.Concluido
            && agendamento.Pagamento?.Status == StatusPagamento.Aprovado;
    }

    public Task<Agendamento?> ObterAgendamentoElegivelParaAvaliacaoAsync(Guid prestadorId, Guid agendamentoId, CancellationToken cancellationToken = default) =>
        Context.Agendamentos
            .Include(a => a.Pagamento)
            .FirstOrDefaultAsync(
                a => a.Id == agendamentoId
                    && a.PrestadorId == prestadorId
                    && a.Status == StatusAgendamento.Concluido
                    && a.Pagamento != null
                    && a.Pagamento.Status == StatusPagamento.Aprovado,
                cancellationToken);
}

public class PagamentoRepository : RepositoryBase<Pagamento>, IPagamentoRepository
{
    public PagamentoRepository(HomeTaskDbContext context) : base(context)
    {
    }

    public override Task<Pagamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Context.Pagamentos
            .Include(p => p.Agendamento)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<Pagamento?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default) =>
        Context.Pagamentos
            .Include(p => p.Agendamento)
            .FirstOrDefaultAsync(p => p.AgendamentoId == agendamentoId, cancellationToken);
}

public class MensagemRepository : RepositoryBase<Mensagem>, IMensagemRepository
{
    public MensagemRepository(HomeTaskDbContext context) : base(context)
    {
    }

    public override Task<Mensagem?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Context.Mensagens
            .Include(m => m.Remetente)
            .Include(m => m.Conversa)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<IEnumerable<Mensagem>> ObterConversaAsync(Guid conversaId, CancellationToken cancellationToken = default) =>
        await Context.Mensagens
            .Include(m => m.Remetente)
            .Include(m => m.Conversa)
            .Where(m => m.ConversaId == conversaId)
            .OrderBy(m => m.DataEnvio)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Mensagem>> ObterPorAgendamentoAsync(Guid agendamentoId, DateTime desde, CancellationToken cancellationToken = default) =>
        await Context.Mensagens
            .Include(m => m.Remetente)
            .Where(m => m.AgendamentoId == agendamentoId && m.DataEnvio >= desde)
            .OrderBy(m => m.DataEnvio)
            .ToListAsync(cancellationToken);

    public Task<List<Mensagem>> ObterUltimasPorAgendamentoAsync(Guid agendamentoId, int quantidade, CancellationToken cancellationToken = default) =>
        Context.Mensagens
            .Where(m => m.AgendamentoId == agendamentoId)
            .OrderByDescending(m => m.DataEnvio)
            .Take(quantidade)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Mensagem>> ObterConversasPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default) =>
        await Context.Mensagens
            .Include(m => m.Remetente)
            .Include(m => m.Conversa)
            .Where(m => m.Conversa != null && (m.Conversa.ClienteId == usuarioId || m.Conversa.PrestadorId == usuarioId))
            .GroupBy(m => m.ConversaId)
            .Select(g => g.OrderByDescending(m => m.DataEnvio).First())
            .OrderByDescending(m => m.DataEnvio)
            .ToListAsync(cancellationToken);

    public Task<int> ObterNaoLidasAsync(Guid usuarioId, CancellationToken cancellationToken = default) =>
        Context.Mensagens
            .Where(m => m.Conversa != null && (m.Conversa.ClienteId == usuarioId || m.Conversa.PrestadorId == usuarioId))
            .Where(m => m.RemetenteId != usuarioId && !m.Lida)
            .CountAsync(cancellationToken);

    public Task<int> RemoverEnviadasAntesDeAsync(DateTime dataLimite, CancellationToken cancellationToken = default) =>
        Context.Mensagens
            .Where(m => m.DataEnvio < dataLimite)
            .ExecuteDeleteAsync(cancellationToken);
}

public class ConversaRepository : RepositoryBase<Conversa>, IConversaRepository
{
    public ConversaRepository(HomeTaskDbContext context) : base(context)
    {
    }

    public override Task<Conversa?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Context.Conversas
            .Include(c => c.Cliente)
                .ThenInclude(cl => cl.Usuario)
            .Include(c => c.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(c => c.Mensagens.OrderBy(m => m.DataEnvio))
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IEnumerable<Conversa>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default) =>
        await Context.Conversas
            .Include(c => c.Cliente)
                .ThenInclude(cl => cl.Usuario)
            .Include(c => c.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(c => c.Mensagens.OrderByDescending(m => m.DataEnvio).Take(1))
            .Where(c => c.Cliente.UsuarioId == usuarioId || c.Prestador.UsuarioId == usuarioId)
            .OrderByDescending(c => c.Mensagens.Max(m => (DateTime?)m.DataEnvio) ?? c.DataCriacao)
            .ToListAsync(cancellationToken);

    public Task<Conversa?> ObterPorClientePrestadorAsync(Guid clienteId, Guid prestadorId, CancellationToken cancellationToken = default) =>
        Context.Conversas
            .FirstOrDefaultAsync(c => c.ClienteId == clienteId && c.PrestadorId == prestadorId, cancellationToken);
}
