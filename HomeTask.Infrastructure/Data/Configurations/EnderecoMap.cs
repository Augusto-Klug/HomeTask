using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HomeTask.Domain.Entidades;

namespace HomeTask.Infrastructure.Data.Configurations;

public class EnderecoMap : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.UsuarioId)
            .IsUnique();

        builder.Property(e => e.Logradouro)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Numero)
            .HasMaxLength(10);

        builder.Property(e => e.Complemento)
            .HasMaxLength(100);

        builder.Property(e => e.Bairro)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Cep)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasOne(e => e.Cidade)
            .WithMany(c => c.Enderecos)
            .HasForeignKey(e => e.CidadeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Usuario)
            .WithOne(u => u.Endereco)
            .HasForeignKey<Endereco>(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Agendamentos)
            .WithOne(a => a.Endereco)
            .HasForeignKey(a => a.EnderecoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
