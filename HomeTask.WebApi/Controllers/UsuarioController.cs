using System.Security.Claims;
using HomeTask.Application.Dtos;
using HomeTask.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTask.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterUsuarioPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(id, cancellationToken);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        [HttpGet]
        public async Task<IActionResult> ObterUsuarioPorEmail([FromQuery] string email, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioService.ObterPorEmailAsync(email, cancellationToken);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> CriarUsuario(UsuarioDto dto, CancellationToken cancellationToken)
        {
            var senha = dto.Senha;
            if (string.IsNullOrEmpty(senha))
                return BadRequest("Senha é obrigatória.");

            if (await _usuarioService.ExisteEmailAsync(dto.Email, cancellationToken))
                return Conflict("Este e-mail já está cadastrado.");

            if (await _usuarioService.ExisteCpfAsync(dto.Documento, cancellationToken))
                return Conflict("Este CPF já está cadastrado.");

            return Ok(await _usuarioService.CriarAsync(dto, senha, cancellationToken));
        }

        [HttpPut]
        public async Task<IActionResult> AtualizarUsuario(UsuarioDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _usuarioService.AtualizarAsync(dto, cancellationToken));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ObterPerfilUsuario(CancellationToken cancellationToken) {

            var IdUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(IdUsuarioClaim, out var idUsuario) || IdUsuarioClaim == null)
            {
                return Unauthorized("Erro ao obter o ID do usuario.");
            }

            var perfil = await _usuarioService.ObterPerfilAsync(idUsuario, cancellationToken);
            return perfil == null ? NotFound() : Ok(perfil);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> AtualizarPrefilUsuario(PerfilDto dto, CancellationToken cancellationToken)
        {
            var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
            {
                return Unauthorized("Erro ao obter o ID do usuario.");
            }

            var sucesso = await _usuarioService.AtualizarPerfilAsync(idUsuario, dto, cancellationToken);

            if (!sucesso)
                return NotFound();
            return Ok();
        }
    }
}
