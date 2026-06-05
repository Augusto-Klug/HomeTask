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
        public async Task<ActionResult<PagamentoDto>> ObterPagamentoPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var pagamento = await _pagamentoService.ObterPorIdAsync(id, cancellationToken);
            if (pagamento == null)
                return NotFound();

            return pagamento;
        }

        [HttpGet]
        public async Task<ActionResult<PagamentoDto>> ObterPagamentoPorAgendamento([FromQuery] Guid agendamentoId, CancellationToken cancellationToken)
        {
            var pagamento = await _pagamentoService.ObterPorAgendamentoAsync(agendamentoId, cancellationToken);
            if (pagamento == null)
                return NotFound();

            return pagamento;
        }

        [HttpPost]
        public async Task<ActionResult<PagamentoDto>> CriarPagamento(PagamentoDto dto, CancellationToken cancellationToken)
        {
            var pagamento = await _pagamentoService.CriarAsync(dto, cancellationToken);
            return pagamento;
        }

        [HttpPost]
        public async Task<ActionResult<PagamentoDto>> ProcessarPagamento(PagamentoDto dto, CancellationToken cancellationToken)
        {
            var pagamento = await _pagamentoService.ProcessarAsync(dto.Id, cancellationToken);
            return pagamento;
        }

        [HttpPost]
        public async Task<ActionResult<PagamentoDto>> ConfirmarPagamento(PagamentoDto dto, CancellationToken cancellationToken)
        {
            var pagamento = await _pagamentoService.ConfirmarAsync(dto.Id, dto.TransacaoId ?? string.Empty, cancellationToken);
            return pagamento;
        }

        [HttpPost]
        public async Task<ActionResult<PagamentoDto>> RecusarPagamento(PagamentoDto dto, CancellationToken cancellationToken)
        {
            var pagamento = await _pagamentoService.RecusarAsync(dto.Id, dto.MotivoRecusa ?? string.Empty, cancellationToken);
            return pagamento;
        }

        [HttpPost]
        public async Task<ActionResult<PagamentoDto>> EstornarPagamento(PagamentoDto dto, CancellationToken cancellationToken)
        {
            var pagamento = await _pagamentoService.EstornarAsync(dto.Id, cancellationToken);
            return pagamento;
        }
    }
}
