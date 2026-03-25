using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HomeTask.Domain.Entities;

namespace HomeTask.Infrastructure.Data.Configurations;

public class ClienteMap : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.Documento).IsUnique();

        builder.HasOne(c => c.Usuario)
            .WithOne(u => u.Cliente)
            .HasForeignKey<Cliente>(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.Documento)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Endereco)
            .HasMaxLength(200);

        builder.Property(c => c.Cidade)
            .HasMaxLength(100);

        builder.Property(c => c.Estado)
            .HasMaxLength(50);

        builder.Property(c => c.Cep)
            .HasMaxLength(10);

        builder.Property(c => c.Bairro)
            .HasMaxLength(100);
    }
}
