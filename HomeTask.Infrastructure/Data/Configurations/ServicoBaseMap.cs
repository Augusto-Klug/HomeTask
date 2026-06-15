using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;

namespace HomeTask.Infrastructure.Data.Configurations;

public class ServicoBaseMap : IEntityTypeConfiguration<ServicoBase>
{
    public void Configure(EntityTypeBuilder<ServicoBase> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Titulo)
            .HasMaxLength(100);

        builder.Property(s => s.Descricao)
            .HasMaxLength(500);

        builder.Property(s => s.UnidadeCobranca)
            .IsRequired();

        builder.Property(s => s.DataCriacao)
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(s => s.PrecoBase)
            .HasColumnType("decimal(10,2)");

        builder.Property(s => s.Categoria)
            .IsRequired();

        builder.HasMany(s => s.AgendamentoServicos)
            .WithOne(a => a.ServicoBase)
            .HasForeignKey(a => a.ServicoBaseId)
            .OnDelete(DeleteBehavior.Restrict);

        // TPH Configuration
        builder.HasDiscriminator<TipoAnuncio>("TipoAnuncio")
            .HasValue<ServicoPrestador>(TipoAnuncio.Oferta)
            .HasValue<ServicoCliente>(TipoAnuncio.Pedido);
    }
}

public class ServicoPrestadorMap : IEntityTypeConfiguration<ServicoPrestador>
{
    public void Configure(EntityTypeBuilder<ServicoPrestador> builder)
    {
        builder.HasOne(s => s.Prestador)
            .WithMany(p => p.ServicosOferecidos)
            .HasForeignKey(s => s.PrestadorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(s => s.MediaAvaliacoes)
            .HasColumnType("decimal(10,2)");
    }
}

public class ServicoClienteMap : IEntityTypeConfiguration<ServicoCliente>
{
    public void Configure(EntityTypeBuilder<ServicoCliente> builder)
    {
        builder.HasOne(s => s.Cliente)
            .WithMany(c => c.ServicosClientes)
            .HasForeignKey(s => s.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
