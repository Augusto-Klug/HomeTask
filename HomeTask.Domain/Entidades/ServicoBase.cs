using HomeTask.Domain.Entities;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Entidades;

public abstract class ServicoBase
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public CategoriaServico Categoria { get; protected set; }
    public DateTime DataCriacao { get; protected set; } = DateTime.UtcNow;
    public bool Ativo { get; protected set; } = true;
    public string Titulo { get; protected set; } = null!;
    public string Descricao { get; protected set; } = null!;
    public decimal PrecoBase { get; protected set; }
    public string UnidadeCobranca { get; protected set; } = null!;
    public TipoAnuncio TipoAnuncio { get; protected set; }

    public ICollection<AgendamentoServico> AgendamentoServicos { get; protected set; } = [];

    public void Ativar() => Ativo = true;
    public void Desativar() => Ativo = false;
}
