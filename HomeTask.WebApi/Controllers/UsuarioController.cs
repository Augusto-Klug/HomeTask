using HomeTask.Application.Interfaces;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces.Usuario;
using Microsoft.AspNetCore.Mvc;

namespace HomeTask.WebApi.Controller
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IConversorUsuario _conversorUsuario;

        public UsuarioController(IUsuarioService usuarioService, IConversorUsuario conversorUsuario)
        {
            _usuarioService = usuarioService;
            _conversorUsuario = conversorUsuario;
        }

        [HttpGet]
        public async Task<IActionResult> ObterUsuarioPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(id, cancellationToken);

            if (usuario == null)
                return NotFound();

            var usuarioContrato = _conversorUsuario.ConverterUsuarioparaContrato(usuario);
            var viewModel = _conversorUsuario.ConverterContratoparaViewModel(usuarioContrato);

            return Ok(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ObterUsuarioPorEmail([FromQuery] string email, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioService.ObterPorEmailAsync(email, cancellationToken);

            if (usuario == null)
                return NotFound();

            var usuarioContrato = _conversorUsuario.ConverterUsuarioparaContrato(usuario);
            var viewModel = _conversorUsuario.ConverterContratoparaViewModel(usuarioContrato);

            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CriarUsuario(UsuarioViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorUsuario.ConverterViewModelparaContrato(viewmodel);
            var usuario = _conversorUsuario.ConverterContratoparaUsuario(contrato);

            if (usuario == null)
                return BadRequest();

            var senha = contrato.Senha;
            if (string.IsNullOrEmpty(senha))
                return BadRequest("Senha é obrigatória.");

            if (await _usuarioService.ExisteEmailAsync(viewmodel.Email, cancellationToken))
                return Conflict("Este e-mail já está cadastrado.");

            if (await _usuarioService.ExisteCpfAsync(viewmodel.Documento, cancellationToken))
                return Conflict("Este CPF já está cadastrado.");

            var usuarioCriado = await _usuarioService.CriarAsync(usuario, senha, cancellationToken);
            var usuarioContrato = _conversorUsuario.ConverterUsuarioparaContrato(usuarioCriado);
            var viewModel = _conversorUsuario.ConverterContratoparaViewModel(usuarioContrato);

            return Ok(viewModel);
        }

        [HttpPut]
        public async Task<IActionResult> AtualizarUsuario(UsuarioViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorUsuario.ConverterViewModelparaContrato(viewmodel);
            var usuario = _conversorUsuario.ConverterContratoparaUsuario(contrato);

            if (usuario == null)
                return BadRequest();

            var usuarioAtualizado = await _usuarioService.AtualizarAsync(usuario, cancellationToken);
            var usuarioContrato = _conversorUsuario.ConverterUsuarioparaContrato(usuarioAtualizado);
            var viewModel = _conversorUsuario.ConverterContratoparaViewModel(usuarioContrato);

            return Ok(viewModel);
        }
    }
}
