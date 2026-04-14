using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HomeTask.Domain.Entities;

namespace HomeTask.Infrastructure.Data.Configurations;

public class ServicoOferecidoMap : IEntityTypeConfiguration<ServicoOferecido>
{
    public void Configure(EntityTypeBuilder<ServicoOferecido> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.Prestador)
            .WithMany(p => p.ServicosOferecidos)
            .HasForeignKey(s => s.PrestadorId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(s => s.Titulo)
            .HasMaxLength(100);

        builder.Property(s => s.Descricao)
            .HasMaxLength(500);

        builder.Property(s => s.UnidadeCobranca)
            .HasMaxLength(20);

        builder.Property(s => s.DataCriacao)
            .HasColumnType("datetime")
            .IsRequired();

         builder.HasOne(s => s.Categoria)
            .WithMany(c => c.ServicosOferecidos)
            .HasForeignKey(s => s.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Cliente)
            .WithMany()
            .HasForeignKey(s => s.ClienteId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(s => s.AgendamentoServicos)
            .WithOne(a => a.ServicoOferecido)
            .HasForeignKey(a => a.ServicoOferecidoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(s => s.PrecoBase)
            .HasColumnType("decimal(10,2)");

        builder.Property(s => s.TipoAnuncio)
            .IsRequired();

    }
}
