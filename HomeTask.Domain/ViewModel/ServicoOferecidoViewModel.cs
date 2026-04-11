using System;
using System.ComponentModel.DataAnnotations;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.ViewModel
{
    public class ServicoOferecidoViewModel
    {
        #region Controle
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? PrestadorId { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Ativo { get; set; }

        #endregion Controle

        #region Dados gerais

        [MaxLength(100)]
        public string Titulo { get; set; }

        [MaxLength(500)]
        public string Descricao { get; set; }

        public CategoriaServico Categoria { get; set; }

        public decimal? Valor { get; set; }

        [MaxLength(20)]
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