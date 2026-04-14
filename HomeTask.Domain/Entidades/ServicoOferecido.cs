using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Entities;
public class ServicoOferecido
{
    public Guid Id { get; private set; }
    public Guid? PrestadorId { get; private set; }
    public Prestador? Prestador { get; private set; }
    public Guid? ClienteId { get; private set; }
    public Cliente? Cliente { get; private set; }
    public Guid CategoriaId { get; private set; }
    public Categoria Categoria { get; private set; } = null!;
    public DateTime DataCriacao { get; private set; }
    public bool Ativo { get; private set; }
    public string Titulo { get; private set; } = null!;
    public string Descricao { get; private set; } = null!;
    public decimal PrecoBase { get; private set; }
    public int? DuracaoEstimadaMinutos { get; private set; }
    public string UnidadeCobranca { get; private set; }
    public TipoAnuncio TipoAnuncio { get; private set; }

    public ICollection<AgendamentoServico> AgendamentoServicos { get; private set; } = [];

    public ServicoOferecido() { }

    public void DefinirDados(
        Guid id,
        Guid? prestadorId,
        Guid categoriaId,
        Guid? clienteId,
        string titulo,
        string descricao,
        decimal precoBase,
        string unidadeCobranca,
        int? duracaoEstimadaMinutos,
        TipoAnuncio tipoAnuncio,
        bool ativo,
        DateTime dataCriacao)
    {
        Id = id;
        PrestadorId = prestadorId;
        CategoriaId = categoriaId;
        ClienteId = clienteId;
        Titulo = titulo;
        Descricao = descricao;
        PrecoBase = precoBase;
        UnidadeCobranca = unidadeCobranca;
        DuracaoEstimadaMinutos = duracaoEstimadaMinutos;
        TipoAnuncio = tipoAnuncio;
        Ativo = ativo;
        DataCriacao = dataCriacao;
    }

    public void Ativar(DateTime dataCriacao)
    {
        DataCriacao = dataCriacao;
        Ativo = true;
    }

    public void Desativar()
    {
        Ativo = false;
    }
}
