using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HomeTask.Domain.Entities;

namespace HomeTask.Infrastructure.Data.Configurations;

public class MensagemMap : IEntityTypeConfiguration<Mensagem>
{
    public void Configure(EntityTypeBuilder<Mensagem> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasOne(m => m.Remetente)
            .WithMany()
            .HasForeignKey(m => m.RemetenteId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(m => m.Conversa)
            .WithMany(c => c.Mensagens)
            .HasForeignKey(m => m.ConversaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Agendamento)
            .WithMany()
            .HasForeignKey(m => m.AgendamentoId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(m => m.Conteudo)
            .IsRequired()
            .HasMaxLength(2000);
    }
}
