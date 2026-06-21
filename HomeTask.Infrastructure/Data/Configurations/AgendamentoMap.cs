using HomeTask.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTask.Infrastructure.Data.Configurations;

public class AgendamentoMap : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.Cliente)
            .WithMany(c => c.Agendamentos)
            .HasForeignKey(a => a.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Prestador)
            .WithMany(p => p.Agendamentos)
            .HasForeignKey(a => a.PrestadorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.PrincipalServicoPrestador)
            .WithMany()
            .HasForeignKey(a => a.PrincipalServicoPrestadorId)
            .OnDelete(DeleteBehavior.Restrict);
        
            builder.HasOne(a => a.Endereco)
            .WithMany(e => e.Agendamentos)
            .HasForeignKey(a => a.EnderecoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.AgendamentoServicos)
            .WithOne(s => s.Agendamento)
            .HasForeignKey(s => s.AgendamentoId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(a => a.Observacoes)
            .HasMaxLength(500);

        builder.Property(a => a.EnderecoDescricao)
            .HasMaxLength(300);

        builder.Property(a => a.ValorTotal)
            .HasColumnType("decimal(10,2)");

        builder.Property(a => a.MotivoRecusa)
            .HasMaxLength(500);
    }
}
