using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HomeTask.Domain.Entities;

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

        builder.HasOne(a => a.ServicoOferecido)
            .WithMany()
            .HasForeignKey(a => a.ServicoOferecidoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(a => a.EnderecoServico)
            .HasMaxLength(300);

        builder.Property(a => a.Observacoes)
            .HasMaxLength(500);

        builder.Property(a => a.ValorTotal)
            .HasColumnType("decimal(10,2)");

        builder.Property(a => a.MotivoRecusa)
            .HasMaxLength(500);
    }
}
