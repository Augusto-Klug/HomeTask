using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HomeTask.Domain.Entities;

namespace HomeTask.Infrastructure.Data.Configurations;

public class CertificacaoMap : IEntityTypeConfiguration<Certificacao>
{
    public void Configure(EntityTypeBuilder<Certificacao> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.Prestador)
            .WithMany(p => p.Certificacoes)
            .HasForeignKey(c => c.PrestadorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Instituicao)
            .HasMaxLength(100);

        builder.Property(c => c.UrlDocumento)
            .HasMaxLength(500);
    }
}
