using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HomeTask.Domain.Entities;

namespace HomeTask.Infrastructure.Data.Configurations;

public class DisponibilidadeMap : IEntityTypeConfiguration<Disponibilidade>
{
    public void Configure(EntityTypeBuilder<Disponibilidade> builder)
    {
        builder.HasKey(d => d.Id);

        builder.HasOne(d => d.Prestador)
            .WithMany(p => p.Disponibilidades)
            .HasForeignKey(d => d.PrestadorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
