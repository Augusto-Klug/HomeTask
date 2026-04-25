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

        public Guid ServicoBaseId { get; private set; }

        public int Quantidade { get; private set; } = 1;
        public decimal ValorUnitario { get; private set; }

         // Navegação
        public Agendamento Agendamento { get; private set; } = null!;
        public ServicoBase ServicoBase { get; private set; } = null!;

        public void DefinirDados(Guid agendamentoId, Guid servicoBaseId, int quantidade, decimal valorUnitario)
        {
            AgendamentoId = agendamentoId;
            ServicoBaseId = servicoBaseId;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
        }
    }
}
