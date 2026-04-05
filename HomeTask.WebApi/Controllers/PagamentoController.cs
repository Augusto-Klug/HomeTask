using HomeTask.Application.Interfaces;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces.Pagamento;
using Microsoft.AspNetCore.Mvc;

namespace HomeTask.WebApi.Controller
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PagamentoController : ControllerBase
    {
        private readonly IPagamentoService _pagamentoService;
        private readonly IConversorPagamento _conversorPagamento;

        public PagamentoController(IPagamentoService pagamentoService, IConversorPagamento conversorPagamento)
        {
            _pagamentoService = pagamentoService;
            _conversorPagamento = conversorPagamento;
        }

        [HttpGet]
        public async Task<IActionResult> ObterPagamentoPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var pagamento = await _pagamentoService.ObterPorIdAsync(id, cancellationToken);

            if (pagamento == null)
                return NotFound();

            var pagamentoContrato = _conversorPagamento.ConverterPagamentoparaContrato(pagamento);
            var viewModel = _conversorPagamento.ConverterContratoparaViewModel(pagamentoContrato);

            return Ok(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ObterPagamentoPorAgendamento([FromQuery] Guid agendamentoId, CancellationToken cancellationToken)
        {
            var pagamento = await _pagamentoService.ObterPorAgendamentoAsync(agendamentoId, cancellationToken);

            if (pagamento == null)
                return NotFound();

            var pagamentoContrato = _conversorPagamento.ConverterPagamentoparaContrato(pagamento);
            var viewModel = _conversorPagamento.ConverterContratoparaViewModel(pagamentoContrato);

            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CriarPagamento(PagamentoViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorPagamento.ConverterViewModelparaContrato(viewmodel);
            var pagamento = _conversorPagamento.ConverterContratoparaPagamento(contrato);

            if (pagamento == null)
                return BadRequest();

            var pagamentoCriado = await _pagamentoService.CriarAsync(pagamento, cancellationToken);
            var pagamentoContrato = _conversorPagamento.ConverterPagamentoparaContrato(pagamentoCriado);
            var viewModel = _conversorPagamento.ConverterContratoparaViewModel(pagamentoContrato);

            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessarPagamento(PagamentoViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorPagamento.ConverterViewModelparaContrato(viewmodel);
            var pagamento = await _pagamentoService.ProcessarAsync(contrato.Id, cancellationToken);
            var pagamentoContrato = _conversorPagamento.ConverterPagamentoparaContrato(pagamento);
            var viewModel = _conversorPagamento.ConverterContratoparaViewModel(pagamentoContrato);

            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarPagamento(PagamentoViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorPagamento.ConverterViewModelparaContrato(viewmodel);
            var pagamento = await _pagamentoService.ConfirmarAsync(contrato.Id, contrato.TransacaoId ?? string.Empty, cancellationToken);
            var pagamentoContrato = _conversorPagamento.ConverterPagamentoparaContrato(pagamento);
            var viewModel = _conversorPagamento.ConverterContratoparaViewModel(pagamentoContrato);

            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RecusarPagamento(PagamentoViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorPagamento.ConverterViewModelparaContrato(viewmodel);
            var pagamento = await _pagamentoService.RecusarAsync(contrato.Id, contrato.MotivoRecusa ?? string.Empty, cancellationToken);
            var pagamentoContrato = _conversorPagamento.ConverterPagamentoparaContrato(pagamento);
            var viewModel = _conversorPagamento.ConverterContratoparaViewModel(pagamentoContrato);

            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EstornarPagamento(PagamentoViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorPagamento.ConverterViewModelparaContrato(viewmodel);
            var pagamento = await _pagamentoService.EstornarAsync(contrato.Id, cancellationToken);
            var pagamentoContrato = _conversorPagamento.ConverterPagamentoparaContrato(pagamento);
            var viewModel = _conversorPagamento.ConverterContratoparaViewModel(pagamentoContrato);

            return Ok(viewModel);
        }
    }
}
