using System.ComponentModel.DataAnnotations;
using HomeTask.Domain.Enums;

namespace HomeTask.Application.Dtos;

public class ServicoPrestadorDto
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
    public FormatoCobranca UnidadeCobranca { get; set; }

    [Required(ErrorMessage = "O preço base é obrigatório")]
    public decimal PrecoBase { get; set; }

    public DateTime DataCriacao { get; set; }
    public int? DuracaoEstimadaMinutos { get; set; }
    public bool AceitaPagamentoAposFinalizacao { get; set; }
    public TipoAnuncio TipoAnuncio { get; set; } = TipoAnuncio.Oferta;
    public string? PrestadorNome { get; set; }
    public string? Logradouro { get; set; }
    public string? Numero { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public decimal? MediaAvaliacoes { get; set; }
    public int TotalAvaliacoes { get; set; }
    public decimal? MediaAvaliacoesPrestador { get; set; }
    public int? TotalAvaliacoesPrestador { get; set; }
}

public class ServicoClienteDto
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
    public DateTime DataCriacao { get; set; }
    public DateTime? DataDesejada { get; set; }
    public TipoAnuncio TipoAnuncio { get; set; } = TipoAnuncio.Pedido;
    public string? ClienteNome { get; set; }
    public string? Logradouro { get; set; }
    public string? Numero { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public decimal? MediaAvaliacoes { get; set; }
    public int TotalAvaliacoes { get; set; }
}

public class ServicoBuscaPaginadaDto
{
    public IReadOnlyCollection<object> Itens { get; init; } = [];
    public int PaginaAtual { get; init; }
    public int TamanhoPagina { get; init; }
    public int TotalRegistros { get; init; }
    public int TotalPaginas { get; init; }
}
