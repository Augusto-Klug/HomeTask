using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HomeTask.Domain.Entidades;

namespace HomeTask.Infrastructure.Data.Configurations;

public class CidadeMap : IEntityTypeConfiguration<Cidade>
{
    public void Configure(EntityTypeBuilder<Cidade> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Estado)
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(c => c.CodIBGE)
            .HasMaxLength(10)
            .HasColumnType("varchar(10)");

        builder.HasMany(c => c.Enderecos)
            .WithOne(e => e.Cidade)
            .HasForeignKey(e => e.CidadeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
