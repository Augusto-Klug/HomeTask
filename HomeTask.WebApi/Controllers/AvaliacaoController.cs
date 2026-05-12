using HomeTask.Application.Dtos;
using HomeTask.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeTask.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AvaliacaoController : ControllerBase
    {
        private readonly IAvaliacaoService _avaliacaoService;

        public AvaliacaoController(IAvaliacaoService avaliacaoService)
        {
            _avaliacaoService = avaliacaoService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterAvaliacaoPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var avaliacao = await _avaliacaoService.ObterPorIdAsync(id, cancellationToken);
            if (avaliacao == null)
                return NotFound();

            return Ok(avaliacao);
        }

        [HttpGet]
        public async Task<IActionResult> ObterAvaliacaoPorAgendamento([FromQuery] Guid agendamentoId, CancellationToken cancellationToken)
        {
            var avaliacao = await _avaliacaoService.ObterPorAgendamentoAsync(agendamentoId, cancellationToken);
            if (avaliacao == null)
                return NotFound();

            return Ok(avaliacao);
        }

        [HttpGet]
        public async Task<IActionResult> ObterAvaliacoesPorPrestador([FromQuery] Guid prestadorId, CancellationToken cancellationToken)
        {
            return Ok(await _avaliacaoService.ObterPorPrestadorAsync(prestadorId, cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> ObterAvaliacoesPorCliente([FromQuery] Guid clienteId, CancellationToken cancellationToken)
        {
            return Ok(await _avaliacaoService.ObterPorClienteAsync(clienteId, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> CriarAvaliacao(AvaliacaoDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _avaliacaoService.CriarAsync(dto, cancellationToken));
        }
    }
}
