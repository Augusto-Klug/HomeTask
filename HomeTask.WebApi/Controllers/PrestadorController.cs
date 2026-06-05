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
        public async Task<ActionResult<PrestadorDto>> ObterPrestadorPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var prestador = await _prestadorService.ObterPorIdAsync(id, cancellationToken);
            if (prestador == null)
                return NotFound();

            return prestador;
        }

        [HttpGet]
        public async Task<ActionResult<PrestadorDto>> ObterPrestadorPorUsuarioId([FromQuery] Guid usuarioId, CancellationToken cancellationToken)
        {
            var prestador = await _prestadorService.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);
            if (prestador == null)
                return NotFound();

            return prestador;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrestadorDto>>> BuscarPrestadores(CategoriaServico? categoria, string? cidade, DateTime? dataDisponivel, CancellationToken cancellationToken)
        {
            var prestadores = await _prestadorService.BuscarAsync(categoria, cidade, dataDisponivel, cancellationToken);
            return prestadores.ToList();
        }

        [HttpPost]
        public async Task<ActionResult<PrestadorDto>> CriarPrestador(PrestadorDto dto, CancellationToken cancellationToken)
        {
            var prestador = await _prestadorService.CriarAsync(dto, cancellationToken);
            return prestador;
        }

        [HttpPut]
        public async Task<ActionResult<PrestadorDto>> AtualizarPrestador(PrestadorDto dto, CancellationToken cancellationToken)
        {
            var prestador = await _prestadorService.AtualizarAsync(dto, cancellationToken);
            return prestador;
        }
    }
}
