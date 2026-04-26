namespace HomeTask.Domain.Contratos;

public class PaginacaoResultado<T>
{
    public IReadOnlyCollection<T> Itens { get; init; } = [];
    public int PaginaAtual { get; init; }
    public int TamanhoPagina { get; init; }
    public int TotalRegistros { get; init; }
    public int TotalPaginas { get; init; }
}
