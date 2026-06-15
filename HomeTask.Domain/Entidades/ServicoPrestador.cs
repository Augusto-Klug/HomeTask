using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Entidades;

public class ServicoPrestador : ServicoBase
{
    public Guid PrestadorId { get; private set; }
    public Prestador Prestador { get; private set; } = null!;
    public int? DuracaoEstimadaMinutos { get; private set; }
    public bool AceitaPagamentoAposFinalizacao { get; private set; }
    public decimal MediaAvaliacoes { get; private set; }
    public int TotalAvaliacoes { get; private set; }
    public ICollection<Avaliacao> Avaliacoes { get; private set; } = [];

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
        FormatoCobranca unidadeCobranca,
        int? duracaoEstimadaMinutos,
        bool aceitaPagamentoAposFinalizacao,
        decimal mediaAvaliacoes,
        int totalAvaliacoes,
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
        MediaAvaliacoes = mediaAvaliacoes;
        TotalAvaliacoes = totalAvaliacoes;
        Ativo = ativo;
        DataCriacao = dataCriacao;
        TipoAnuncio = TipoAnuncio.Oferta;
    }

    public void AtualizarMetricasAvaliacao(decimal mediaAvaliacoes, int totalAvaliacoes)
    {
        MediaAvaliacoes = mediaAvaliacoes;
        TotalAvaliacoes = totalAvaliacoes;
    }
}
