using HomeTask.Application.Interfaces;
using HomeTask.Application.Dtos;
using HomeTask.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HomeTask.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PrestadorController : ControllerBase
    {
        private readonly IPrestadorService _prestadorService;

        public PrestadorController(IPrestadorService prestadorService)
        {
            _prestadorService = prestadorService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterPrestadorPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var prestador = await _prestadorService.ObterPorIdAsync(id, cancellationToken);
            if (prestador == null)
                return NotFound();

            return Ok(prestador);
        }

        [HttpGet]
        public async Task<IActionResult> ObterPrestadorPorUsuarioId([FromQuery] Guid usuarioId, CancellationToken cancellationToken)
        {
            var prestador = await _prestadorService.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);
            if (prestador == null)
                return NotFound();

            return Ok(prestador);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarPrestadores(CategoriaServico? categoria, string? cidade, DateTime? dataDisponivel, CancellationToken cancellationToken)
        {
            return Ok(await _prestadorService.BuscarAsync(categoria, cidade, dataDisponivel, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> CriarPrestador(PrestadorDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _prestadorService.CriarAsync(dto, cancellationToken));
        }

        [HttpPut]
        public async Task<IActionResult> AtualizarPrestador(PrestadorDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _prestadorService.AtualizarAsync(dto, cancellationToken));
        }
    }
}
