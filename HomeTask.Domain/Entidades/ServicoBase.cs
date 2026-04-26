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
    public FormatoCobranca UnidadeCobranca { get; protected set; }
    public TipoAnuncio TipoAnuncio { get; protected set; }

    public ICollection<AgendamentoServico> AgendamentoServicos { get; protected set; } = [];

    public void Ativar() => Ativo = true;
    public void Desativar() => Ativo = false;
}
