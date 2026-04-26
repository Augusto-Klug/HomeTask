using HomeTask.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTask.Infrastructure.Data.Configurations;

public class PagamentoMap : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasOne(p => p.Agendamento)
            .WithOne(a => a.Pagamento)
            .HasForeignKey<Pagamento>(p => p.AgendamentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.Valor)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(p => p.TransacaoId)
            .HasMaxLength(100);

        builder.Property(p => p.MotivoRecusa)
            .HasMaxLength(500);
    }
}
