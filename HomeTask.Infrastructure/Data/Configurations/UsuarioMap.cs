using HomeTask.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTask.Infrastructure.Data.Configurations;

public class UsuarioMap : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Documento).IsUnique();

        builder.Property(u => u.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.SenhaHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.Documento)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.Telefone)
            .HasMaxLength(15);

        builder.Property(u => u.TipoUsuario)
            .IsRequired();


        builder.Property(u => u.UltimoAcesso)
            .IsRequired(false);

    }
}
