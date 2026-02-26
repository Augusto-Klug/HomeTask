using System;
using System.ComponentModel.DataAnnotations;

namespace HomeTask.Domain.Contratos
{
    public class AvaliacaoContrato
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AgendamentoId { get; set; }
        public Guid ClienteId { get; set; }
        public Guid PrestadorId { get; set; }

        [Range(0, 5)]
        public int Nota { get; set; }

        [MaxLength(1000)]
        public string? Comentario { get; set; }

        public DateTime DataAvaliacao { get; set; }

        public bool Visivel { get; set; } = true;
    }
}
