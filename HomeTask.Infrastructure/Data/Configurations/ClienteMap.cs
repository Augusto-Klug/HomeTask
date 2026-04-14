using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HomeTask.Domain.Entities;

namespace HomeTask.Infrastructure.Data.Configurations;

public class ClienteMap : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.Usuario)
            .WithOne(u => u.Cliente)
            .HasForeignKey<Cliente>(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Conversas)
            .WithOne(cv => cv.Cliente)
            .HasForeignKey(cv => cv.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}
