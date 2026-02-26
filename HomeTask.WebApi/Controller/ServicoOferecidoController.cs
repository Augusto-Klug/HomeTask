using HomeTask.Application.Interfaces;
using HomeTask.Domain.Enums;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces.ServicoOferecido;
using Microsoft.AspNetCore.Mvc;

namespace HomeTask.WebApi.Controller
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ServicoOferecidoController : ControllerBase
    {
        private readonly IServicoService _servicoService;
        private readonly IConversorServicoOferecido _conversorServico;

        public ServicoOferecidoController(IServicoService servicoService, IConversorServicoOferecido conversorServico)
        {
            _servicoService = servicoService;
            _conversorServico = conversorServico;
        }

        [HttpGet]
        public async Task<IActionResult> ObterServicoPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var servico = await _servicoService.ObterPorIdAsync(id, cancellationToken);

            if (servico == null)
                return NotFound();

            var servicoContrato = _conversorServico.ConverterServicoOferecidoparaContrato(servico);
            var viewModel = _conversorServico.ConverterContratoparaViewModel(servicoContrato);

            return Ok(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ObterServicosPorPrestador([FromQuery] Guid prestadorId, CancellationToken cancellationToken)
        {
            var servicos = await _servicoService.ObterPorPrestadorAsync(prestadorId, cancellationToken);

            var viewModels = servicos.Select(s =>
            {
                var c = _conversorServico.ConverterServicoOferecidoparaContrato(s);
                return _conversorServico.ConverterContratoparaViewModel(c);
            });

            return Ok(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarServicos(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken)
        {
            var servicos = await _servicoService.BuscarAsync(categoria, cidade, precoMaximo, cancellationToken);

            var viewModels = servicos.Select(s =>
            {
                var c = _conversorServico.ConverterServicoOferecidoparaContrato(s);
                return _conversorServico.ConverterContratoparaViewModel(c);
            });

            return Ok(viewModels);
        }

        [HttpPost]
        public async Task<IActionResult> CriarServico(ServicoOferecidoViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorServico.ConverterViewModelparaContrato(viewmodel);
            var servico = _conversorServico.ConverterContratoparaServicoOferecido(contrato);

            if (servico == null)
                return BadRequest();

            var servicoCriado = await _servicoService.CriarAsync(servico, cancellationToken);
            var servicoContrato = _conversorServico.ConverterServicoOferecidoparaContrato(servicoCriado);
            var viewModel = _conversorServico.ConverterContratoparaViewModel(servicoContrato);

            return Ok(viewModel);
        }

        [HttpPut]
        public async Task<IActionResult> AtualizarServico(ServicoOferecidoViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorServico.ConverterViewModelparaContrato(viewmodel);
            var servico = _conversorServico.ConverterContratoparaServicoOferecido(contrato);

            if (servico == null)
                return BadRequest();

            var servicoAtualizado = await _servicoService.AtualizarAsync(servico, cancellationToken);
            var servicoContrato = _conversorServico.ConverterServicoOferecidoparaContrato(servicoAtualizado);
            var viewModel = _conversorServico.ConverterContratoparaViewModel(servicoContrato);

            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RemoverServico(ServicoOferecidoViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorServico.ConverterViewModelparaContrato(viewmodel);
            var removido = await _servicoService.RemoverAsync(contrato.Id, cancellationToken);

            if (!removido)
                return NotFound();

            return Ok();
        }
    }
}
