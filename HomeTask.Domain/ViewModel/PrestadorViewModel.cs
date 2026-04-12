using System.ComponentModel.DataAnnotations;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.ViewModel
{
    public class PrestadorViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UsuarioId { get; set; }

        [MaxLength(1000)]
        public string? Descricao { get; set; }

        public int? RaioAtendimentoKm { get; set; }
        public StatusPrestador Status { get; set; }
        public decimal MediaAvaliacoes { get; set; }
        public int TotalAvaliacoes { get; set; }
        public int TotalServicosConcluidos { get; set; }
        public DateTime? DataVerificacao { get; set; }
    }
}
