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
        public Guid? ClienteId { get; set; }
        public Guid CategoriaId { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Ativo { get; set; }

        #endregion Controle

        #region Dados gerais

        [MaxLength(100)]
        public string Titulo { get; set; }

        [MaxLength(500)]
        public string Descricao { get; set; }

        [MaxLength(20)]
        public string UnidadeCobranca { get; set; }
        public decimal PrecoBase { get; set; }
        public int? DuracaoEstimadaMinutos { get; set; }
        public TipoAnuncio TipoAnuncio { get; set; }

        #endregion Dados gerais

        #region Agendamento prestador 

        public bool? AceitaPagamentoAposFinalizacao { get; set; }

        #endregion Agendamento prestador
    }

}