using HomeTask.Application.Dtos;
using HomeTask.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeTask.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PagamentoController : ControllerBase
    {
        private readonly IPagamentoService _pagamentoService;

        public PagamentoController(IPagamentoService pagamentoService)
        {
            _pagamentoService = pagamentoService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterPagamentoPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var pagamento = await _pagamentoService.ObterPorIdAsync(id, cancellationToken);
            if (pagamento == null)
                return NotFound();

            return Ok(pagamento);
        }

        [HttpGet]
        public async Task<IActionResult> ObterPagamentoPorAgendamento([FromQuery] Guid agendamentoId, CancellationToken cancellationToken)
        {
            var pagamento = await _pagamentoService.ObterPorAgendamentoAsync(agendamentoId, cancellationToken);
            if (pagamento == null)
                return NotFound();

            return Ok(pagamento);
        }

        [HttpPost]
        public async Task<IActionResult> CriarPagamento(PagamentoDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _pagamentoService.CriarAsync(dto, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> ProcessarPagamento(PagamentoDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _pagamentoService.ProcessarAsync(dto.Id, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarPagamento(PagamentoDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _pagamentoService.ConfirmarAsync(dto.Id, dto.TransacaoId ?? string.Empty, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> RecusarPagamento(PagamentoDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _pagamentoService.RecusarAsync(dto.Id, dto.MotivoRecusa ?? string.Empty, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> EstornarPagamento(PagamentoDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _pagamentoService.EstornarAsync(dto.Id, cancellationToken));
        }
    }
}
