using Microsoft.EntityFrameworkCore;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Enums;
using HomeTask.Infrastructure.Data;
using HomeTask.Domain.Entidades;

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
                .ThenInclude(e => e.Cidade)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(s => s.ServicoBase)
            .Include(a => a.Pagamento)
            .Include(a => a.Avaliacao)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Agendamento> CriarAsync(Agendamento agendamento, List<Guid> servicosIds, CancellationToken cancellationToken = default)
    {
        var servicos = await _context.Servicos
            .Where(s => servicosIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        if (servicos == null || !servicos.Any())
            throw new InvalidOperationException("Nenhum serviço válido encontrado para o agendamento.");

        decimal valorTotal = 0;
        int duracaoTotal = 0;
        Guid prestadorId = agendamento.PrestadorId;
        Guid clienteId = agendamento.ClienteId;

        foreach (var servico in servicos)
        {
            if (servico is ServicoPrestador sp)
            {
                if (prestadorId == Guid.Empty || prestadorId == default)
                {
                    prestadorId = sp.PrestadorId;
                }
                duracaoTotal += sp.DuracaoEstimadaMinutos ?? 0;
            }

            if (servico is ServicoCliente sc)
            {
                if (clienteId == Guid.Empty || clienteId == default)
                {
                    clienteId = sc.ClienteId;
                }
            }

            var agendamentoServico = new AgendamentoServico();
            agendamentoServico.DefinirDados(agendamento.Id, servico.Id, 1, servico.PrecoBase);
            agendamento.AdicionarServico(agendamentoServico);

            valorTotal += servico.PrecoBase;
        }

        // Busca endereço principal se não informado
        Guid enderecoId = agendamento.EnderecoId;
        if (enderecoId == Guid.Empty || enderecoId == default)
        {
            var enderecoUsuario = await _context.Enderecos
                .FirstOrDefaultAsync(e => e.Usuario.Cliente != null && e.Usuario.Cliente.Id == clienteId, cancellationToken);

            if (enderecoUsuario != null)
            {
                enderecoId = enderecoUsuario.Id;
            }
        }

        if (enderecoId == Guid.Empty || enderecoId == default)
            throw new InvalidOperationException("Cliente não possui endereço cadastrado para o agendamento.");

        // Atualiza os dados do agendamento com o que foi calculado/descoberto
        agendamento.DefinirDados(
            agendamento.Id,
            clienteId,
            prestadorId,
            agendamento.DataHoraAgendada,
            duracaoTotal > 0 ? duracaoTotal : agendamento.DuracaoMinutos,
            StatusAgendamento.Solicitado,
            enderecoId,
            agendamento.Observacoes,
            valorTotal,
            DateTime.UtcNow,
            null,
            null,
            null
        );

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
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Endereco)
                .ThenInclude(e => e.Cidade)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(s => s.ServicoBase)
            .Where(a => a.ClienteId == clienteId)
            .OrderByDescending(a => a.DataHoraAgendada)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.Endereco)
                .ThenInclude(e => e.Cidade)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(s => s.ServicoBase)
            .Where(a => a.PrestadorId == prestadorId)
            .OrderByDescending(a => a.DataHoraAgendada)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterSolicitacoesPendentesPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.Endereco)
                .ThenInclude(e => e.Cidade)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(s => s.ServicoBase)
            .Where(a => a.PrestadorId == prestadorId && a.Status == StatusAgendamento.Solicitado)
            .OrderByDescending(a => a.DataSolicitacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterPorStatusAsync(StatusAgendamento status, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Prestador)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(s => s.ServicoBase)
            .Where(a => a.Status == status)
            .OrderByDescending(a => a.DataSolicitacao)
            .ToListAsync(cancellationToken);
    }
}
