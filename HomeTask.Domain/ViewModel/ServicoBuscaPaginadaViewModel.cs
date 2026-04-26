namespace HomeTask.Domain.ViewModel;

public class ServicoBuscaPaginadaViewModel
{
    public IReadOnlyCollection<object> Itens { get; init; } = [];
    public int PaginaAtual { get; init; }
    public int TamanhoPagina { get; init; }
    public int TotalRegistros { get; init; }
    public int TotalPaginas { get; init; }
}
