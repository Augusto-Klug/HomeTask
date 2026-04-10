using System;
using System.ComponentModel.DataAnnotations;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Contratos
{
    public class ServicoOferecidoContrato
    {
        #region Controle
        public Guid Id { get; set; }
        public Guid PrestadorId { get; set; }
        public DateTime DataCriacao { get; set; } 
        public bool Ativo { get; set; }

        #endregion Controle

        #region Dados gerais
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public CategoriaServico Categoria { get; set; }
        public decimal? Valor { get; set; }
        public string UnidadeCobranca { get; set; }
        #endregion Dados gerais

        #region Agendamento cliente
        public DateOnly? DataAgendamento { get; set; }

        #endregion Agendamento cliente

        #region Agendamento prestador 

        public bool? AceitaPagamentoAposFinalizacao { get; set; }

        #endregion Agendamento prestador
    }
}
