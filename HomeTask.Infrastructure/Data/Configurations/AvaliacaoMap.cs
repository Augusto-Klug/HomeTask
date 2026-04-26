using HomeTask.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTask.Infrastructure.Data.Configurations;

public class AvaliacaoMap : IEntityTypeConfiguration<Avaliacao>
{
    public void Configure(EntityTypeBuilder<Avaliacao> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasOne(av => av.Agendamento)
            .WithOne(a => a.Avaliacao)
            .HasForeignKey<Avaliacao>(av => av.AgendamentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(av => av.Cliente)
            .WithMany(c => c.Avaliacoes)
            .HasForeignKey(av => av.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(av => av.Prestador)
            .WithMany(p => p.Avaliacoes)
            .HasForeignKey(av => av.PrestadorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(a => a.Comentario)
            .HasMaxLength(1000);
    }
}
