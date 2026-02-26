using HomeTask.Application.Interfaces;
using HomeTask.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomeTask.Web.Pages.Servicos;

public class DetalhesModel : PageModel
{
    private readonly IServicoService _servicoService;
    private readonly IAvaliacaoService _avaliacaoService;

    public DetalhesModel(IServicoService servicoService, IAvaliacaoService avaliacaoService)
    {
        _servicoService = servicoService;
        _avaliacaoService = avaliacaoService;
    }

    public ServicoDetalheViewModel Servico { get; set; } = null!;

    public List<AvaliacaoItemViewModel> Avaliacoes { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var servico = await _servicoService.ObterPorIdAsync(id);

        if (servico == null)
            return NotFound();

        Servico = new ServicoDetalheViewModel
        {
            Id = servico.Id,
            Titulo = servico.Titulo ?? ObterNomeCategoria(servico.Categoria),
            Descricao = servico.Descricao ?? string.Empty,
            CategoriaNome = ObterNomeCategoria(servico.Categoria),
            PrecoBase = servico.PrecoBase,
            UnidadeCobranca = servico.UnidadeCobranca,
            DuracaoEstimadaMinutos = servico.DuracaoEstimadaMinutos,
            PrestadorId = servico.PrestadorId,
            PrestadorNome = servico.Prestador.Usuario.Nome,
            Cidade = servico.Prestador.Cidade ?? string.Empty,
            Estado = servico.Prestador.Estado ?? string.Empty,
            MediaAvaliacoes = servico.Prestador.MediaAvaliacoes,
            TotalAvaliacoes = servico.Prestador.TotalAvaliacoes,
            TotalServicosConcluidos = servico.Prestador.TotalServicosConcluidos
        };

        var avaliacoes = await _avaliacaoService.ObterPorPrestadorAsync(servico.PrestadorId);
        Avaliacoes = avaliacoes
            .Where(a => a.Visivel)
            .OrderByDescending(a => a.DataAvaliacao)
            .Take(10)
            .Select(a => new AvaliacaoItemViewModel
            {
                Nota = a.Nota,
                Comentario = a.Comentario,
                DataAvaliacao = a.DataAvaliacao
            })
            .ToList();

        return Page();
    }

    private static string ObterNomeCategoria(CategoriaServico categoria) => categoria switch
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

    public class ServicoDetalheViewModel
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string CategoriaNome { get; set; } = string.Empty;
        public decimal PrecoBase { get; set; }
        public string UnidadeCobranca { get; set; } = string.Empty;
        public int DuracaoEstimadaMinutos { get; set; }
        public Guid PrestadorId { get; set; }
        public string PrestadorNome { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public decimal MediaAvaliacoes { get; set; }
        public int TotalAvaliacoes { get; set; }
        public int TotalServicosConcluidos { get; set; }
    }

    public class AvaliacaoItemViewModel
    {
        public int Nota { get; set; }
        public string? Comentario { get; set; }
        public DateTime DataAvaliacao { get; set; }
    }
}
