using HomeTask.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTask.Infrastructure.Data.Configurations;

public class PortfolioMap : IEntityTypeConfiguration<Portfolio>
{
    public void Configure(EntityTypeBuilder<Portfolio> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasOne(po => po.Prestador)
            .WithMany(p => p.Portfolios)
            .HasForeignKey(po => po.PrestadorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(po => po.Titulo)
            .HasMaxLength(100);

        builder.Property(po => po.Descricao)
            .HasMaxLength(500);

        builder.Property(po => po.UrlImagem)
            .IsRequired()
            .HasMaxLength(500);
    }
}
