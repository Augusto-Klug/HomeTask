using HomeTask.Application.Interfaces;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeTask.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AvaliacaoController : ControllerBase
    {
        private readonly IAvaliacaoService _avaliacaoService;
        private readonly IConversorAvaliacao _conversorAvaliacao;

        public AvaliacaoController(IAvaliacaoService avaliacaoService, IConversorAvaliacao conversorAvaliacao)
        {
            _avaliacaoService = avaliacaoService;
            _conversorAvaliacao = conversorAvaliacao;
        }

        [HttpGet]
        public async Task<IActionResult> ObterAvaliacaoPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var avaliacao = await _avaliacaoService.ObterPorIdAsync(id, cancellationToken);

            if (avaliacao == null)
                return NotFound();

            var avaliacaoContrato = _conversorAvaliacao.ConverterAvaliacaoparaContrato(avaliacao);
            var viewModel = _conversorAvaliacao.ConverterContratoparaViewModel(avaliacaoContrato);

            return Ok(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ObterAvaliacaoPorAgendamento([FromQuery] Guid agendamentoId, CancellationToken cancellationToken)
        {
            var avaliacao = await _avaliacaoService.ObterPorAgendamentoAsync(agendamentoId, cancellationToken);

            if (avaliacao == null)
                return NotFound();

            var avaliacaoContrato = _conversorAvaliacao.ConverterAvaliacaoparaContrato(avaliacao);
            var viewModel = _conversorAvaliacao.ConverterContratoparaViewModel(avaliacaoContrato);

            return Ok(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ObterAvaliacoesPorPrestador([FromQuery] Guid prestadorId, CancellationToken cancellationToken)
        {
            var avaliacoes = await _avaliacaoService.ObterPorPrestadorAsync(prestadorId, cancellationToken);

            var viewModels = avaliacoes.Select(a =>
            {
                var c = _conversorAvaliacao.ConverterAvaliacaoparaContrato(a);
                return _conversorAvaliacao.ConverterContratoparaViewModel(c);
            });

            return Ok(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> ObterAvaliacoesPorCliente([FromQuery] Guid clienteId, CancellationToken cancellationToken)
        {
            var avaliacoes = await _avaliacaoService.ObterPorClienteAsync(clienteId, cancellationToken);

            var viewModels = avaliacoes.Select(a =>
            {
                var c = _conversorAvaliacao.ConverterAvaliacaoparaContrato(a);
                return _conversorAvaliacao.ConverterContratoparaViewModel(c);
            });

            return Ok(viewModels);
        }

        [HttpPost]
        public async Task<IActionResult> CriarAvaliacao(AvaliacaoViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorAvaliacao.ConverterViewModelparaContrato(viewmodel);
            var avaliacao = _conversorAvaliacao.ConverterContratoparaAvaliacao(contrato);

            if (avaliacao == null)
                return BadRequest();

            var avaliacaoCriada = await _avaliacaoService.CriarAsync(avaliacao, cancellationToken);
            var avaliacaoContrato = _conversorAvaliacao.ConverterAvaliacaoparaContrato(avaliacaoCriada);
            var viewModel = _conversorAvaliacao.ConverterContratoparaViewModel(avaliacaoContrato);

            return Ok(viewModel);
        }
    }
}
