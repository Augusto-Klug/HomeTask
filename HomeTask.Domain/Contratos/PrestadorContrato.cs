using System;
using System.ComponentModel.DataAnnotations;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Contratos
{
    public class PrestadorContrato
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UsuarioId { get; set; }

        [MaxLength(1000)]
        public string? Descricao { get; set; }

        [MaxLength(200)]
        public string? Endereco { get; set; }

        [MaxLength(100)]
        public string? Cidade { get; set; }

        [MaxLength(50)]
        public string? Estado { get; set; }

        [MaxLength(10)]
        public string? Cep { get; set; }

        [MaxLength(100)]
        public string? Bairro { get; set; }

        public int RaioAtendimentoKm { get; set; } = 10;

        public StatusPrestador Status { get; set; }

        public decimal MediaAvaliacoes { get; set; }

        public int TotalAvaliacoes { get; set; }

        public int TotalServicosConcluidos { get; set; }

        public DateTime? DataVerificacao { get; set; }
    }
}
