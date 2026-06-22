using HomeTask.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTask.Infrastructure.Data.Configurations;

public class PrestadorMap : IEntityTypeConfiguration<Prestador>
{
    public void Configure(EntityTypeBuilder<Prestador> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasOne(p => p.Usuario)
            .WithOne(u => u.Prestador)
            .HasForeignKey<Prestador>(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.Descricao)
            .HasMaxLength(1000);

        builder.HasMany(p => p.Conversas)
            .WithOne(c => c.Prestador)
            .HasForeignKey(c => c.PrestadorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.MediaAvaliacoes)
            .HasColumnType("decimal(3,2)");

        builder.Property(p => p.DataPrimeiraNotificacaoBaixaAvaliacao);
        builder.Property(p => p.TotalAvaliacoesNaNotificacao);
        builder.Property(p => p.DataInicioSuspensao);
        builder.Property(p => p.DataFimSuspensao);
    }
}
