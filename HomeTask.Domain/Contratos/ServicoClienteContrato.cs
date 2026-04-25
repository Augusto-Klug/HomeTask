using System;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Contratos
{
    public class ServicoClienteContrato
    {
        public Guid Id { get; set; }
        public Guid? ClienteId { get; set; }
        public CategoriaServico Categoria { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Ativo { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string UnidadeCobranca { get; set; } = string.Empty;
        public decimal PrecoBase { get; set; }
        public DateTime? DataDesejada { get; set; }
        public TipoAnuncio TipoAnuncio { get; set; } = TipoAnuncio.Pedido;

        public string? ClienteNome { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
    }
}
