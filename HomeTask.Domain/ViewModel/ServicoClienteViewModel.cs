using System;
using System.ComponentModel.DataAnnotations;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.ViewModel
{
    public class ServicoClienteViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? ClienteId { get; set; }
        public CategoriaServico Categoria { get; set; }
        public bool Ativo { get; set; } = true;

        [Required(ErrorMessage = "O título é obrigatório")]
        [MaxLength(100)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [MaxLength(500)]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "A unidade de cobrança é obrigatória")]
        public FormatoCobranca UnidadeCobranca { get; set; }

        public decimal PrecoBase { get; set; }
        
        public DateTime? DataDesejada { get; set; }

        public string? ClienteNome { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
    }
}
