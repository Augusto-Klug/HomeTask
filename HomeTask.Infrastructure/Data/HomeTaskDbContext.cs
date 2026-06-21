using Microsoft.EntityFrameworkCore;
using HomeTask.Domain.Entidades;

namespace HomeTask.Infrastructure.Data;

public class HomeTaskDbContext : DbContext
{
    public HomeTaskDbContext(DbContextOptions<HomeTaskDbContext> options) : base(options)
    {
    }

    // Usuários
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Auth> Auth => Set<Auth>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Prestador> Prestadores => Set<Prestador>();

    // Localização
    public DbSet<Cidade> Cidades => Set<Cidade>();
    public DbSet<Endereco> Enderecos => Set<Endereco>();

    // Serviços
    public DbSet<ServicoBase> Servicos => Set<ServicoBase>();
    public DbSet<ServicoPrestador> ServicosPrestadores => Set<ServicoPrestador>();
    public DbSet<ServicoCliente> ServicosClientes => Set<ServicoCliente>();
    public DbSet<Disponibilidade> Disponibilidades => Set<Disponibilidade>();
    public DbSet<Certificacao> Certificacoes => Set<Certificacao>();
    public DbSet<Portfolio> Portfolios => Set<Portfolio>();

    // Negócio
    public DbSet<Agendamento> Agendamentos => Set<Agendamento>();
    public DbSet<AgendamentoServico> AgendamentoServicos => Set<AgendamentoServico>();
    public DbSet<Pagamento> Pagamentos => Set<Pagamento>();
    public DbSet<Avaliacao> Avaliacoes => Set<Avaliacao>();
    public DbSet<AvaliacaoCliente> AvaliacoesClientes => Set<AvaliacaoCliente>();

    // Comunicação
    public DbSet<Conversa> Conversas => Set<Conversa>();
    public DbSet<Mensagem> Mensagens => Set<Mensagem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HomeTaskDbContext).Assembly);
    }
}
