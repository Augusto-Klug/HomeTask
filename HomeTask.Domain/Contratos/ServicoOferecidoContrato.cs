using System;
using System.ComponentModel.DataAnnotations;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Contratos
{
    public class ServicoOferecidoContrato
    {
        public Guid Id { get; set; }
        public Guid? PrestadorId { get; set; }
        public Guid? ClienteId { get; set; }
        public Guid CategoriaId { get; set; }
        public DateTime DataCriacao { get; set; } 
        public bool Ativo { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string UnidadeCobranca { get; set; }
        public decimal PrecoBase { get; set; }
        public int? DuracaoEstimadaMinutos { get; set; }
        public TipoAnuncio TipoAnuncio { get; set; }

    }
}
