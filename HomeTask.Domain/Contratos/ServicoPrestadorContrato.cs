using System;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Contratos
{
    public class ServicoPrestadorContrato
    {
        public Guid Id { get; set; }
        public Guid? PrestadorId { get; set; }
        public CategoriaServico Categoria { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Ativo { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public FormatoCobranca UnidadeCobranca { get; set; }
        public decimal PrecoBase { get; set; }
        public int? DuracaoEstimadaMinutos { get; set; }
        public bool AceitaPagamentoAposFinalizacao { get; set; }
        public TipoAnuncio TipoAnuncio { get; set; } = TipoAnuncio.Oferta;

        public string? PrestadorNome { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public decimal? MediaAvaliacoes { get; set; }
    }
}
