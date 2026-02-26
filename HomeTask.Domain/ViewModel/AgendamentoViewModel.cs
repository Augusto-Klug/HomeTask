using System;
using System.ComponentModel.DataAnnotations;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.ViewModel
{
    public class AgendamentoViewModel
    {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid ClienteId { get; set; }
            public Guid PrestadorId { get; set; }
            public Guid ServicoOferecidoId { get; set; }

            public DateTime DataHoraAgendada { get; set; }

            public int DuracaoMinutos { get; set; }

            public StatusAgendamento Status { get; set; }

            [MaxLength(300)]
            public string? EnderecoServico { get; set; }

            [MaxLength(500)]
            public string? Observacoes { get; set; }

            public decimal ValorTotal { get; set; }

            public DateTime DataSolicitacao { get; set; }

            public DateTime? DataResposta { get; set; }

            public DateTime? DataConclusao { get; set; }

            [MaxLength(500)]
            public string? MotivoRecusa { get; set; }
    }
}
