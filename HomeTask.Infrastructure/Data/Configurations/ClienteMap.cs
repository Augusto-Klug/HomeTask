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

        builder.Property(c => c.TipoUsuario)
            .IsRequired();

        builder.Property(c => c.Documento)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Endereco)
            .IsRequired(false)
            .HasMaxLength(200);

        builder.Property(c => c.Cidade)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(c => c.Estado)
            .IsRequired(false)
            .HasMaxLength(2);

        builder.Property(c => c.Cep)
            .IsRequired(false)
            .HasMaxLength(10);

        builder.Property(c => c.Bairro)
            .IsRequired(false)
            .HasMaxLength(50);
    }
}
