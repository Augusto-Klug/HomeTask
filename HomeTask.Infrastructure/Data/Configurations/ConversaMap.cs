using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HomeTask.Domain.Entidades;

namespace HomeTask.Infrastructure.Data.Configurations;

public class ConversaMap : IEntityTypeConfiguration<Conversa>
{
    public void Configure(EntityTypeBuilder<Conversa> builder)
    {
        builder.HasKey(c => c.Id);

        // Garante 1 conversa por par cliente-prestador
        builder.HasIndex(c => new { c.ClienteId, c.PrestadorId })
            .IsUnique();

        builder.Property(c => c.DataCriacao)
            .IsRequired();

        builder.HasOne(c => c.Cliente)
            .WithMany(cl => cl.Conversas)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Prestador)
            .WithMany(p => p.Conversas)
            .HasForeignKey(c => c.PrestadorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Mensagens)
            .WithOne(m => m.Conversa)
            .HasForeignKey(m => m.ConversaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
