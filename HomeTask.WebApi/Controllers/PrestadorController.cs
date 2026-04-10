using HomeTask.Application.Interfaces;
using HomeTask.Domain.Enums;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeTask.WebApi.Controller
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PrestadorController : ControllerBase
    {
        private readonly IPrestadorService _prestadorService;
        private readonly IConversorPrestador _conversorPrestador;

        public PrestadorController(IPrestadorService prestadorService, IConversorPrestador conversorPrestador)
        {
            _prestadorService = prestadorService;
            _conversorPrestador = conversorPrestador;
        }

        [HttpGet]
        public async Task<IActionResult> ObterPrestadorPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var prestador = await _prestadorService.ObterPorIdAsync(id, cancellationToken);

            if (prestador == null)
                return NotFound();

            var prestadorContrato = _conversorPrestador.ConverterPrestadorparaContrato(prestador);
            var viewModel = _conversorPrestador.ConverterContratoparaViewModel(prestadorContrato);

            return Ok(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ObterPrestadorPorUsuarioId([FromQuery] Guid usuarioId, CancellationToken cancellationToken)
        {
            var prestador = await _prestadorService.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);

            if (prestador == null)
                return NotFound();

            var prestadorContrato = _conversorPrestador.ConverterPrestadorparaContrato(prestador);
            var viewModel = _conversorPrestador.ConverterContratoparaViewModel(prestadorContrato);

            return Ok(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarPrestadores(CategoriaServico? categoria, string? cidade, DateTime? dataDisponivel, CancellationToken cancellationToken)
        {
            var prestadores = await _prestadorService.BuscarAsync(categoria, cidade, dataDisponivel, cancellationToken);

            var viewModels = prestadores.Select(p =>
            {
                var contrato = _conversorPrestador.ConverterPrestadorparaContrato(p);
                return _conversorPrestador.ConverterContratoparaViewModel(contrato);
            });

            return Ok(viewModels);
        }

        [HttpPost]
        public async Task<IActionResult> CriarPrestador(PrestadorViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorPrestador.ConverterViewModelparaContrato(viewmodel);
            var prestador = _conversorPrestador.ConverterContratoparaPrestador(contrato);

            if (prestador == null)
                return BadRequest();

            var prestadorCriado = await _prestadorService.CriarAsync(prestador, cancellationToken);
            var prestadorContrato = _conversorPrestador.ConverterPrestadorparaContrato(prestadorCriado);
            var viewModel = _conversorPrestador.ConverterContratoparaViewModel(prestadorContrato);

            return Ok(viewModel);
        }

        [HttpPut]
        public async Task<IActionResult> AtualizarPrestador(PrestadorViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorPrestador.ConverterViewModelparaContrato(viewmodel);
            var prestador = _conversorPrestador.ConverterContratoparaPrestador(contrato);

            if (prestador == null)
                return BadRequest();

            var prestadorAtualizado = await _prestadorService.AtualizarAsync(prestador, cancellationToken);
            var prestadorContrato = _conversorPrestador.ConverterPrestadorparaContrato(prestadorAtualizado);
            var viewModel = _conversorPrestador.ConverterContratoparaViewModel(prestadorContrato);

            return Ok(viewModel);
        }
    }
}
