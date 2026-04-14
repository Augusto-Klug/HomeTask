using HomeTask.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeTask.Domain.Entidades
{
    public class AgendamentoServico
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid AgendamentoId { get; set; }

        public Guid ServicoOferecidoId { get; set; }

        public int Quantidade { get; set; } = 1;
        public decimal ValorUnitario { get; set; }

         // Navegação
        public Agendamento Agendamento { get; set; } = null!;
        public ServicoOferecido ServicoOferecido { get; set; } = null!;

    }
}
