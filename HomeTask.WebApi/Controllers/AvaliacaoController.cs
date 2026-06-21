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
        private readonly IAvaliacaoClienteService _avaliacaoClienteService;

        public AvaliacaoController(IAvaliacaoService avaliacaoService, IAvaliacaoClienteService avaliacaoClienteService)
        {
            _avaliacaoService = avaliacaoService;
            _avaliacaoClienteService = avaliacaoClienteService;
        }

        [HttpGet]
        public async Task<ActionResult<AvaliacaoDto>> ObterAvaliacaoPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var avaliacao = await _avaliacaoService.ObterPorIdAsync(id, cancellationToken);
            if (avaliacao == null)
                return NotFound();

            return avaliacao;
        }

        [HttpGet]
        public async Task<ActionResult<AvaliacaoDto>> ObterAvaliacaoPorAgendamento([FromQuery] Guid agendamentoId, CancellationToken cancellationToken)
        {
            var avaliacao = await _avaliacaoService.ObterPorAgendamentoAsync(agendamentoId, cancellationToken);
            if (avaliacao == null)
                return NotFound();

            return avaliacao;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AvaliacaoDto>>> ObterAvaliacoesPorPrestador([FromQuery] Guid prestadorId, CancellationToken cancellationToken)
        {
            var avaliacoes = await _avaliacaoService.ObterPorPrestadorAsync(prestadorId, cancellationToken);
            return avaliacoes.ToList();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AvaliacaoDto>>> ObterAvaliacoesPorCliente([FromQuery] Guid clienteId, CancellationToken cancellationToken)
        {
            var avaliacoes = await _avaliacaoService.ObterPorClienteAsync(clienteId, cancellationToken);
            return avaliacoes.ToList();
        }

        [HttpPost]
        public async Task<ActionResult<AvaliacaoDto>> CriarAvaliacao(AvaliacaoDto dto, CancellationToken cancellationToken)
        {
            var avaliacao = await _avaliacaoService.CriarAsync(dto, cancellationToken);
            return avaliacao;
        }

        [HttpGet]
        public async Task<ActionResult<AvaliacaoClienteDto>> ObterAvaliacaoClientePorAgendamento([FromQuery] Guid agendamentoId, CancellationToken cancellationToken)
        {
            var avaliacao = await _avaliacaoClienteService.ObterPorAgendamentoAsync(agendamentoId, cancellationToken);
            if (avaliacao == null)
                return NotFound();

            return avaliacao;
        }

        [HttpPost]
        public async Task<ActionResult<AvaliacaoClienteDto>> CriarAvaliacaoCliente(AvaliacaoClienteDto dto, CancellationToken cancellationToken)
        {
            var avaliacao = await _avaliacaoClienteService.CriarAsync(dto, cancellationToken);
            return avaliacao;
        }
    }
}
