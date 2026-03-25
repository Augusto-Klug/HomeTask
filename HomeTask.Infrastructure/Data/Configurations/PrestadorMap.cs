using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HomeTask.Domain.Entities;

namespace HomeTask.Infrastructure.Data.Configurations;

public class PrestadorMap : IEntityTypeConfiguration<Prestador>
{
    public void Configure(EntityTypeBuilder<Prestador> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.Documento).IsUnique();

        builder.HasOne(p => p.Usuario)
            .WithOne(u => u.Prestador)
            .HasForeignKey<Prestador>(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.Descricao)
            .HasMaxLength(1000);

        builder.Property(p => p.Endereco)
            .HasMaxLength(200);

        builder.Property(p => p.Cidade)
            .HasMaxLength(100);

        builder.Property(p => p.Estado)
            .HasMaxLength(50);

        builder.Property(p => p.Cep)
            .HasMaxLength(10);

        builder.Property(u => u.Documento)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Bairro)
            .HasMaxLength(100);

        builder.Property(p => p.MediaAvaliacoes)
            .HasColumnType("decimal(3,2)");
    }
}
