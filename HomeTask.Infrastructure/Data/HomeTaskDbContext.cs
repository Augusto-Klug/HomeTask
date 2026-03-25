using Microsoft.EntityFrameworkCore;
using HomeTask.Domain.Entities;

namespace HomeTask.Infrastructure.Data;
public class HomeTaskDbContext : DbContext
{
    public HomeTaskDbContext(DbContextOptions<HomeTaskDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Prestador> Prestadores => Set<Prestador>();
    public DbSet<ServicoOferecido> ServicosOferecidos => Set<ServicoOferecido>();
    public DbSet<Agendamento> Agendamentos => Set<Agendamento>();
    public DbSet<Pagamento> Pagamentos => Set<Pagamento>();
    public DbSet<Avaliacao> Avaliacoes => Set<Avaliacao>();
    public DbSet<Certificacao> Certificacoes => Set<Certificacao>();
    public DbSet<Portfolio> Portfolios => Set<Portfolio>();
    public DbSet<Disponibilidade> Disponibilidades => Set<Disponibilidade>();
    public DbSet<Mensagem> Mensagens => Set<Mensagem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HomeTaskDbContext).Assembly);
    }
}
