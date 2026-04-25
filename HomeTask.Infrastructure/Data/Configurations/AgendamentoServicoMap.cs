using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using HomeTask.Domain.Entidades;

namespace HomeTask.Infrastructure.Data.Configurations;

public class AgendamentoServicoMap : IEntityTypeConfiguration<AgendamentoServico>
{
    public void Configure(EntityTypeBuilder<AgendamentoServico> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Quantidade)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(a => a.ValorUnitario)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.HasOne(a => a.Agendamento)
            .WithMany(ag => ag.AgendamentoServicos)
            .HasForeignKey(a => a.AgendamentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.ServicoBase)
            .WithMany(s => s.AgendamentoServicos)
            .HasForeignKey(a => a.ServicoBaseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
