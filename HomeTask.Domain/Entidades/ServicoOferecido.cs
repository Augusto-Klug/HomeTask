using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Entities;
public class ServicoOferecido
{
    public Guid Id { get; set; }
    public Guid? PrestadorId { get; set; }
    public Prestador? Prestador { get; set; }
    public Guid? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public Guid CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;
    public DateTime DataCriacao { get; set; }
    public bool Ativo { get; set; }
    public string Titulo { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public decimal PrecoBase { get; set; }
    public int? DuracaoEstimadaMinutos { get; set; }
    public string UnidadeCobranca { get; set; }
    public TipoAnuncio TipoAnuncio { get; set; }

    public ICollection<AgendamentoServico> AgendamentoServicos { get; set; } = [];

    public ServicoOferecido() { }
}
