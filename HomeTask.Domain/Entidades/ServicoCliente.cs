using HomeTask.Domain.Entities;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Entidades;

public class ServicoCliente : ServicoBase
{
    public Guid ClienteId { get; private set; }
    public Cliente Cliente { get; private set; } = null!;
    public DateTime? DataDesejada { get; private set; }

    public ServicoCliente() 
    {
        TipoAnuncio = TipoAnuncio.Pedido;
    }

    public void DefinirDados(
        Guid id,
        Guid clienteId,
        CategoriaServico categoria,
        string titulo,
        string descricao,
        decimal precoBase,
        string unidadeCobranca,
        DateTime? dataDesejada,
        bool ativo,
        DateTime dataCriacao)
    {
        Id = id;
        ClienteId = clienteId;
        Categoria = categoria;
        Titulo = titulo;
        Descricao = descricao;
        PrecoBase = precoBase;
        UnidadeCobranca = unidadeCobranca;
        DataDesejada = dataDesejada;
        Ativo = ativo;
        DataCriacao = dataCriacao;
        TipoAnuncio = TipoAnuncio.Pedido;
    }
}
