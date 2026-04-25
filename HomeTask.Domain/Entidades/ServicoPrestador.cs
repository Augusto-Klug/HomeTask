using HomeTask.Domain.Entities;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Entidades;

public class ServicoPrestador : ServicoBase
{
    public Guid PrestadorId { get; private set; }
    public Prestador Prestador { get; private set; } = null!;
    public int? DuracaoEstimadaMinutos { get; private set; }
    public bool AceitaPagamentoAposFinalizacao { get; private set; }

    public ServicoPrestador() 
    {
        TipoAnuncio = TipoAnuncio.Oferta;
    }

    public void DefinirDados(
        Guid id,
        Guid prestadorId,
        CategoriaServico categoria,
        string titulo,
        string descricao,
        decimal precoBase,
        string unidadeCobranca,
        int? duracaoEstimadaMinutos,
        bool aceitaPagamentoAposFinalizacao,
        bool ativo,
        DateTime dataCriacao)
    {
        Id = id;
        PrestadorId = prestadorId;
        Categoria = categoria;
        Titulo = titulo;
        Descricao = descricao;
        PrecoBase = precoBase;
        UnidadeCobranca = unidadeCobranca;
        DuracaoEstimadaMinutos = duracaoEstimadaMinutos;
        AceitaPagamentoAposFinalizacao = aceitaPagamentoAposFinalizacao;
        Ativo = ativo;
        DataCriacao = dataCriacao;
        TipoAnuncio = TipoAnuncio.Oferta;
    }
}
