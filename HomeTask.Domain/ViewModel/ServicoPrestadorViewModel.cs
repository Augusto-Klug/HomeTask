using System;
using System.ComponentModel.DataAnnotations;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.ViewModel
{
    public class ServicoPrestadorViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? PrestadorId { get; set; }
        public CategoriaServico Categoria { get; set; }
        public bool Ativo { get; set; } = true;

        [Required(ErrorMessage = "O título é obrigatório")]
        [MaxLength(100)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [MaxLength(500)]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "A unidade de cobrança é obrigatória")]
        [MaxLength(20)]
        public string UnidadeCobranca { get; set; } = string.Empty;

        [Required(ErrorMessage = "O preço base é obrigatório")]
        public decimal PrecoBase { get; set; }
        
        public int? DuracaoEstimadaMinutos { get; set; }
        public bool AceitaPagamentoAposFinalizacao { get; set; }

        public string? PrestadorNome { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public decimal? MediaAvaliacoes { get; set; }
    }
}
