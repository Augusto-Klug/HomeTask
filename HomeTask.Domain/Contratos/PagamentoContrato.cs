using System;
using System.ComponentModel.DataAnnotations;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Contratos
{
    public class PagamentoContrato
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AgendamentoId { get; set; }

        public decimal Valor { get; set; }

        public TipoPagamento TipoPagamento { get; set; }

        public StatusPagamento Status { get; set; }

        [MaxLength(100)]
        public string? TransacaoId { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataProcessamento { get; set; }

        public DateTime? DataConfirmacao { get; set; }

        [MaxLength(500)]
        public string? MotivoRecusa { get; set; }
    }
}
