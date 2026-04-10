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
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(s => s.Titulo)
            .HasMaxLength(100);

        builder.Property(s => s.Descricao)
            .HasMaxLength(500);

        builder.Property(s => s.Valor)
            .HasColumnType("decimal(10,2)");

        builder.Property(s => s.Categoria)
            .HasColumnType("int")
            .IsRequired();

        builder.Property(s => s.UnidadeCobranca)
            .HasMaxLength(20);

        builder.Property(s => s.DataAgendamento)
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(s => s.AceitaPagamentoAposFinalizacao)
            .HasColumnType("bit")
            .IsRequired(false);

        builder.Property(s => s.DataCriacao)
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(s => s.Ativo)
            .HasColumnType("bit")
            .IsRequired();


    }
}
