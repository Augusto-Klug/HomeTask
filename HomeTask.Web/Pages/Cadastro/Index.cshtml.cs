using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HomeTask.Domain.Entities;
using HomeTask.Domain.Enums;
using HomeTask.Application.Interfaces;

namespace HomeTask.Web.Pages.Cadastro;

public class IndexModel : PageModel
{
    private readonly IUsuarioService _usuarioService;
    private readonly IClienteService _clienteService;
    private readonly IPrestadorService _prestadorService;

    public IndexModel(
        IUsuarioService usuarioService,
        IClienteService clienteService,
        IPrestadorService prestadorService)
    {
        _usuarioService = usuarioService;
        _clienteService = clienteService;
        _prestadorService = prestadorService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Verificar se e-mail já existe
        if (await _usuarioService.ExisteEmailAsync(Input.Email))
        {
            ModelState.AddModelError("Input.Email", "Este e-mail já está cadastrado.");
            return Page();
        }

        // Verificar se CPF já existe
        if (await _usuarioService.ExisteCpfAsync(Input.Cpf))
        {
            ModelState.AddModelError("Input.Cpf", "Este CPF já está cadastrado.");
            return Page();
        }

        // Criar usuário
        var usuario = new Usuario
        {
            Nome = Input.Nome,
            Email = Input.Email,
            Cpf = Input.Cpf,
            Telefone = Input.Telefone,
            Tipo = (TipoUsuario)Input.TipoUsuario
        };

        await _usuarioService.CriarAsync(usuario, Input.Senha);

        // Criar Cliente ou Prestador baseado no tipo
        if (usuario.Tipo == TipoUsuario.Cliente)
        {
            var cliente = new Cliente
            {
                UsuarioId = usuario.Id,
                Endereco = Input.Endereco,
                Cidade = Input.Cidade,
                Estado = Input.Estado,
                Cep = Input.Cep,
                Bairro = Input.Bairro
            };
            await _clienteService.CriarAsync(cliente);
        }
        else
        {
            var prestador = new Prestador
            {
                UsuarioId = usuario.Id,
                Endereco = Input.Endereco,
                Cidade = Input.Cidade,
                Estado = Input.Estado,
                Cep = Input.Cep,
                Bairro = Input.Bairro,
                Status = StatusPrestador.EmAnalise
            };
            await _prestadorService.CriarAsync(prestador);
        }

        // TODO: Enviar e-mail de confirmação
        // TODO: Autenticar usuário automaticamente

        return RedirectToPage("/Cadastro/Sucesso");
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Selecione o tipo de cadastro")]
        public int TipoUsuario { get; set; } = 1;

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [StringLength(14, ErrorMessage = "CPF inválido")]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Telefone inválido")]
        public string? Telefone { get; set; }

        public string? Endereco { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Cep { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "A senha deve ter entre 8 e 100 caracteres")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme a senha")]
        [Compare("Senha", ErrorMessage = "As senhas não conferem")]
        public string ConfirmarSenha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Você precisa aceitar os termos de uso")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Você precisa aceitar os termos de uso")]
        public bool AceitaTermos { get; set; }
    }
}
