using HomeTask.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeTask.Domain.Entidades
{
    public class AgendamentoServico
    {
        public AgendamentoServico() { }

        public Guid Id { get; private set; } = Guid.NewGuid();

        public Guid AgendamentoId { get; private set; }

        public Guid ServicoOferecidoId { get; private set; }

        public int Quantidade { get; private set; } = 1;
        public decimal ValorUnitario { get; private set; }

         // Navegação
        public Agendamento Agendamento { get; private set; } = null!;
        public ServicoOferecido ServicoOferecido { get; private set; } = null!;

    }
}
