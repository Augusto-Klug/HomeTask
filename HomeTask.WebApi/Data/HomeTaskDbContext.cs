using Microsoft.EntityFrameworkCore;
using HomeTask.WebApi.Models.Entities;

namespace HomeTask.WebApi.Data;

/// <summary>
/// Contexto do banco de dados HomeTask (RNF02 - MySQL)
/// </summary>
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

        // Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Cpf).IsUnique();
        });

        // Cliente - Usuario (1:1)
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasOne(c => c.Usuario)
                .WithOne(u => u.Cliente)
                .HasForeignKey<Cliente>(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Prestador - Usuario (1:1)
        modelBuilder.Entity<Prestador>(entity =>
        {
            entity.HasOne(p => p.Usuario)
                .WithOne(u => u.Prestador)
                .HasForeignKey<Prestador>(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ServicoOferecido - Prestador
        modelBuilder.Entity<ServicoOferecido>(entity =>
        {
            entity.HasOne(s => s.Prestador)
                .WithMany(p => p.ServicosOferecidos)
                .HasForeignKey(s => s.PrestadorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Agendamento
        modelBuilder.Entity<Agendamento>(entity =>
        {
            entity.HasOne(a => a.Cliente)
                .WithMany(c => c.Agendamentos)
                .HasForeignKey(a => a.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Prestador)
                .WithMany(p => p.Agendamentos)
                .HasForeignKey(a => a.PrestadorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.ServicoOferecido)
                .WithMany()
                .HasForeignKey(a => a.ServicoOferecidoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Pagamento - Agendamento (1:1)
        modelBuilder.Entity<Pagamento>(entity =>
        {
            entity.HasOne(p => p.Agendamento)
                .WithOne(a => a.Pagamento)
                .HasForeignKey<Pagamento>(p => p.AgendamentoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Avaliacao - Agendamento (1:1)
        modelBuilder.Entity<Avaliacao>(entity =>
        {
            entity.HasOne(av => av.Agendamento)
                .WithOne(a => a.Avaliacao)
                .HasForeignKey<Avaliacao>(av => av.AgendamentoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(av => av.Cliente)
                .WithMany(c => c.Avaliacoes)
                .HasForeignKey(av => av.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(av => av.Prestador)
                .WithMany(p => p.Avaliacoes)
                .HasForeignKey(av => av.PrestadorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Certificacao - Prestador
        modelBuilder.Entity<Certificacao>(entity =>
        {
            entity.HasOne(c => c.Prestador)
                .WithMany(p => p.Certificacoes)
                .HasForeignKey(c => c.PrestadorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Portfolio - Prestador
        modelBuilder.Entity<Portfolio>(entity =>
        {
            entity.HasOne(po => po.Prestador)
                .WithMany(p => p.Portfolios)
                .HasForeignKey(po => po.PrestadorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Disponibilidade - Prestador
        modelBuilder.Entity<Disponibilidade>(entity =>
        {
            entity.HasOne(d => d.Prestador)
                .WithMany(p => p.Disponibilidades)
                .HasForeignKey(d => d.PrestadorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Mensagem
        modelBuilder.Entity<Mensagem>(entity =>
        {
            entity.HasOne(m => m.Remetente)
                .WithMany()
                .HasForeignKey(m => m.RemetenteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Destinatario)
                .WithMany()
                .HasForeignKey(m => m.DestinatarioId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Agendamento)
                .WithMany()
                .HasForeignKey(m => m.AgendamentoId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
