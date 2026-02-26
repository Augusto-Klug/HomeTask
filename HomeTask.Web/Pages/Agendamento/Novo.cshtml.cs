using System.ComponentModel.DataAnnotations;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomeTask.Web.Pages.Agendamento;

public class NovoModel : PageModel
{
    private readonly IAgendamentoService _agendamentoService;
    private readonly IServicoService _servicoService;
    private readonly IClienteService _clienteService;

    public NovoModel(
        IAgendamentoService agendamentoService,
        IServicoService servicoService,
        IClienteService clienteService)
    {
        _agendamentoService = agendamentoService;
        _servicoService = servicoService;
        _clienteService = clienteService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public ServicoResumoViewModel Servico { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(Guid servicoId, Guid prestadorId)
    {
        var usuarioId = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioId))
            return RedirectToPage("/Login/Index", new { returnUrl = $"/Agendamento/Novo?servicoId={servicoId}&prestadorId={prestadorId}" });

        var servico = await _servicoService.ObterPorIdAsync(servicoId);
        if (servico == null)
            return NotFound();

        Servico = new ServicoResumoViewModel
        {
            Id = servico.Id,
            Titulo = servico.Titulo ?? servico.Categoria.ToString(),
            PrecoBase = servico.PrecoBase,
            UnidadeCobranca = servico.UnidadeCobranca,
            PrestadorNome = servico.Prestador.Usuario.Nome
        };

        Input.ServicoOferecidoId = servicoId;
        Input.PrestadorId = prestadorId;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
            return RedirectToPage("/Login/Index");

        if (!ModelState.IsValid)
        {
            var servico = await _servicoService.ObterPorIdAsync(Input.ServicoOferecidoId);
            if (servico != null)
            {
                Servico = new ServicoResumoViewModel
                {
                    Id = servico.Id,
                    Titulo = servico.Titulo ?? servico.Categoria.ToString(),
                    PrecoBase = servico.PrecoBase,
                    UnidadeCobranca = servico.UnidadeCobranca,
                    PrestadorNome = servico.Prestador.Usuario.Nome
                };
            }
            return Page();
        }

        var cliente = await _clienteService.ObterPorUsuarioIdAsync(Guid.Parse(usuarioIdStr));
        if (cliente == null)
            return RedirectToPage("/Index");

        var agendamento = new HomeTask.Domain.Entities.Agendamento
        {
            ClienteId = cliente.Id,
            PrestadorId = Input.PrestadorId,
            ServicoOferecidoId = Input.ServicoOferecidoId,
            DataHoraAgendada = Input.DataHoraAgendada,
            EnderecoServico = Input.EnderecoServico,
            Observacoes = Input.Observacoes
        };

        await _agendamentoService.CriarAsync(agendamento);

        return RedirectToPage("/Agendamento/Sucesso");
    }

    public class InputModel
    {
        public Guid ServicoOferecidoId { get; set; }
        public Guid PrestadorId { get; set; }

        [Required(ErrorMessage = "Selecione a data e hora")]
        public DateTime DataHoraAgendada { get; set; }

        [MaxLength(300)]
        public string? EnderecoServico { get; set; }

        [MaxLength(500)]
        public string? Observacoes { get; set; }
    }

    public class ServicoResumoViewModel
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public decimal PrecoBase { get; set; }
        public string UnidadeCobranca { get; set; } = string.Empty;
        public string PrestadorNome { get; set; } = string.Empty;
    }
}
