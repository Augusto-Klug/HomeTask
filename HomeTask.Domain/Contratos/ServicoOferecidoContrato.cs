using System;
using System.ComponentModel.DataAnnotations;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Contratos
{
    public class ServicoOferecidoContrato
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PrestadorId { get; set; }

        public CategoriaServico Categoria { get; set; }

        [MaxLength(100)]
        public string? Titulo { get; set; }

        [MaxLength(500)]
        public string? Descricao { get; set; }

        public decimal PrecoBase { get; set; }

        [MaxLength(20)]
        public string UnidadeCobranca { get; set; } = "hora";

        public int DuracaoEstimadaMinutos { get; set; }

        public bool Ativo { get; set; } = true;

        public DateTime DataCriacao { get; set; }
    }
}
