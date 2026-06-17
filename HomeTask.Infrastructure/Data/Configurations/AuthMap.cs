using HomeTask.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTask.Infrastructure.Data.Configurations;

public class AuthMap : IEntityTypeConfiguration<Auth>
{
    public void Configure(EntityTypeBuilder<Auth> builder)
    {
        builder.ToTable("Auth");

        builder.HasKey(a => a.Id);

        builder.HasIndex(a => a.UsuarioId).IsUnique();
        builder.HasIndex(a => a.ResetarSenhaToken).IsUnique();

        builder.Property(a => a.ResetarSenhaToken)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(a => a.ResetarSenhaTokenExpiraEm)
            .IsRequired();

        builder.HasOne(a => a.Usuario)
            .WithOne(u => u.Auth)
            .HasForeignKey<Auth>(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
