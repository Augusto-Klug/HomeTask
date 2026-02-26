using System.ComponentModel.DataAnnotations;
using HomeTask.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomeTask.Web.Pages.Login;

public class IndexModel : PageModel
{
    private readonly IUsuarioService _usuarioService;

    public IndexModel(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErroLogin { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var senhaValida = await _usuarioService.ValidarSenhaAsync(Input.Email, Input.Senha);

        if (!senhaValida)
        {
            ErroLogin = "E-mail ou senha inválidos.";
            return Page();
        }

        var usuario = await _usuarioService.ObterPorEmailAsync(Input.Email);

        HttpContext.Session.SetString("UsuarioId", usuario!.Id.ToString());
        HttpContext.Session.SetString("UsuarioNome", usuario.Nome);
        HttpContext.Session.SetString("UsuarioTipo", usuario.Tipo.ToString());

        return RedirectToPage("/Index");
    }

    public class InputModel
    {
        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        public string Senha { get; set; } = string.Empty;
    }
}
