using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;
using HomeTask.Domain.Repositories;
using HomeTask.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeTask.Infrastructure.Repositories;

public class UsuarioRepository : RepositoryBase<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(HomeTaskDbContext context) : base(context)
    {
    }

    public override Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Context.Usuarios
            .Include(u => u.Cliente)
            .Include(u => u.Prestador)
            .Include(u => u.Endereco)
                .ThenInclude(e => e.Cidade)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default) =>
        Context.Usuarios
            .Include(u => u.Cliente)
            .Include(u => u.Prestador)
            .Include(u => u.Endereco)
                .ThenInclude(e => e.Cidade)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken = default) =>
        Context.Usuarios.AnyAsync(u => u.Email == email, cancellationToken);

    public Task<bool> ExisteCpfAsync(string cpf, CancellationToken cancellationToken = default) =>
        Context.Usuarios.AnyAsync(u => u.Documento == cpf, cancellationToken);
}

public class ClienteRepository : RepositoryBase<Cliente>, IClienteRepository
{
    public ClienteRepository(HomeTaskDbContext context) : base(context)
    {
    }

    public override Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Context.Clientes
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<Cliente?> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default) =>
        Context.Clientes
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId, cancellationToken);

    public async Task<IEnumerable<Agendamento>> ObterHistoricoAgendamentosAsync(Guid clienteId, CancellationToken cancellationToken = default) =>
        await Context.Agendamentos
            .Include(a => a.Prestador)
                .ThenInclude(p => p.Usuario)
            .Include(a => a.Endereco)
                .ThenInclude(e => e.Cidade)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(s => s.ServicoBase)
            .Include(a => a.Avaliacao)
            .Include(a => a.Pagamento)
            .Where(a => a.ClienteId == clienteId)
            .OrderByDescending(a => a.DataHoraAgendada)
            .ToListAsync(cancellationToken);
}

public class PrestadorRepository : RepositoryBase<Prestador>, IPrestadorRepository
{
    public PrestadorRepository(HomeTaskDbContext context) : base(context)
    {
    }

    public override Task<Prestador?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Context.Prestadores
            .Include(p => p.Usuario)
                .ThenInclude(u => u.Endereco)
                    .ThenInclude(e => e.Cidade)
            .Include(p => p.ServicosOferecidos)
            .Include(p => p.Certificacoes)
            .Include(p => p.Portfolios)
            .Include(p => p.Disponibilidades)
            .Include(p => p.Avaliacoes)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<Prestador?> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default) =>
        Context.Prestadores
            .Include(p => p.Usuario)
                .ThenInclude(u => u.Endereco)
                    .ThenInclude(e => e.Cidade)
            .Include(p => p.ServicosOferecidos)
            .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId, cancellationToken);

    public async Task<IEnumerable<Prestador>> BuscarAsync(CategoriaServico? categoria, string? cidade, DateTime? dataDisponivel, CancellationToken cancellationToken = default)
    {
        var query = Context.Prestadores
            .Include(p => p.Usuario)
                .ThenInclude(u => u.Endereco)
                    .ThenInclude(e => e.Cidade)
            .Include(p => p.ServicosOferecidos)
            .Include(p => p.Avaliacoes)
            .Include(p => p.Disponibilidades)
            .Where(p => p.Status == StatusPrestador.Ativo);

        if (categoria.HasValue)
        {
            query = query.Where(p => p.ServicosOferecidos.Any(s => s.Categoria == categoria.Value && s.Ativo));
        }

        if (!string.IsNullOrWhiteSpace(cidade))
        {
            query = query.Where(p => p.Usuario.Endereco != null && p.Usuario.Endereco.Cidade.Nome.Contains(cidade));
        }

        if (dataDisponivel.HasValue)
        {
            var diaSemana = (int)dataDisponivel.Value.DayOfWeek;
            var hora = dataDisponivel.Value.TimeOfDay;

            query = query.Where(p => p.Disponibilidades.Any(d =>
                d.DiaSemana == diaSemana &&
                d.HoraInicio <= hora &&
                d.HoraFim >= hora &&
                d.Ativo));
        }

        return await query.OrderByDescending(p => p.MediaAvaliacoes).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterHistoricoServicosAsync(Guid prestadorId, CancellationToken cancellationToken = default) =>
        await Context.Agendamentos
            .Include(a => a.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Endereco)
                .ThenInclude(e => e.Cidade)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(s => s.ServicoBase)
            .Include(a => a.Avaliacao)
            .Where(a => a.PrestadorId == prestadorId)
            .OrderByDescending(a => a.DataHoraAgendada)
            .ToListAsync(cancellationToken);

    public Task<Prestador?> ObterComAvaliacoesAsync(Guid prestadorId, CancellationToken cancellationToken = default) =>
        Context.Prestadores
            .Include(p => p.Avaliacoes)
            .FirstOrDefaultAsync(p => p.Id == prestadorId, cancellationToken);
}

public class CidadeRepository : RepositoryBase<Cidade>, ICidadeRepository
{
    public CidadeRepository(HomeTaskDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Cidade>> ListarAsync(CancellationToken cancellationToken = default) =>
        await Context.Cidades
            .OrderBy(c => c.Estado)
            .ThenBy(c => c.Nome)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Cidade>> BuscarAsync(string termo, CancellationToken cancellationToken = default) =>
        await Context.Cidades
            .Where(c => c.Nome.Contains(termo) || c.Estado.Contains(termo))
            .OrderBy(c => c.Estado)
            .ThenBy(c => c.Nome)
            .ToListAsync(cancellationToken);
}
