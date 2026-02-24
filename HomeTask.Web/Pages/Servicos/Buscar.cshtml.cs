using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using HomeTask.Domain.Enums;
using HomeTask.Application.Interfaces;

namespace HomeTask.Web.Pages.Servicos;

public class BuscarModel : PageModel
{
    private readonly IServicoService _servicoService;

    public BuscarModel(IServicoService servicoService)
    {
        _servicoService = servicoService;
    }

    [BindProperty(SupportsGet = true)]
    public FiltroModel Filtro { get; set; } = new();

    public List<ServicoViewModel> Servicos { get; set; } = [];

    public SelectList Categorias { get; set; } = null!;

    public async Task OnGetAsync()
    {
        // Preencher lista de categorias
        var categorias = Enum.GetValues<CategoriaServico>()
            .Select(c => new { Id = (int)c, Nome = ObterNomeCategoria(c) })
            .ToList();
        Categorias = new SelectList(categorias, "Id", "Nome");

        // Buscar serviços
        CategoriaServico? categoria = Filtro.Categoria.HasValue 
            ? (CategoriaServico)Filtro.Categoria.Value 
            : null;

        var servicosDb = await _servicoService.BuscarAsync(categoria, Filtro.Cidade, Filtro.PrecoMaximo);

        Servicos = servicosDb
            .Where(s => !Filtro.AvaliacaoMinima.HasValue || s.Prestador.MediaAvaliacoes >= Filtro.AvaliacaoMinima.Value)
            .Select(s => new ServicoViewModel
            {
                Id = s.Id,
                Titulo = s.Titulo ?? ObterNomeCategoria(s.Categoria),
                Descricao = s.Descricao ?? "",
                CategoriaNome = ObterNomeCategoria(s.Categoria),
                PrecoBase = s.PrecoBase,
                PrestadorNome = s.Prestador.Usuario.Nome,
                Cidade = s.Prestador.Cidade ?? "",
                MediaAvaliacoes = s.Prestador.MediaAvaliacoes,
                TotalAvaliacoes = s.Prestador.TotalAvaliacoes
            })
            .ToList();
    }

    private static string ObterNomeCategoria(CategoriaServico categoria)
    {
        return categoria switch
        {
            CategoriaServico.Faxina => "Faxina",
            CategoriaServico.Jardinagem => "Jardinagem",
            CategoriaServico.Reparos => "Reparos",
            CategoriaServico.Lavanderia => "Lavanderia",
            CategoriaServico.Passadoria => "Passadoria",
            CategoriaServico.Babysitter => "Babá",
            CategoriaServico.CuidadorIdosos => "Cuidador de Idosos",
            CategoriaServico.PetSitter => "Pet Sitter",
            CategoriaServico.Cozinheiro => "Cozinheiro",
            CategoriaServico.ServicosGerais => "Serviços Gerais",
            _ => categoria.ToString()
        };
    }

    public class FiltroModel
    {
        public int? Categoria { get; set; }
        public string? Cidade { get; set; }
        public decimal? PrecoMaximo { get; set; }
        public int? AvaliacaoMinima { get; set; }
    }

    public class ServicoViewModel
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string CategoriaNome { get; set; } = string.Empty;
        public decimal PrecoBase { get; set; }
        public string PrestadorNome { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public decimal MediaAvaliacoes { get; set; }
        public int TotalAvaliacoes { get; set; }
    }
}
