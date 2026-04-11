using HomeTask.Application.Interfaces;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace HomeTask.WebApi.Controller
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly IConversorCliente _conversorCliente;

        public ClienteController(IClienteService clienteService, IConversorCliente conversorCliente)
        {
            _clienteService = clienteService;
            _conversorCliente = conversorCliente;
        }

        [HttpGet]
        public async Task<IActionResult> ObterClientePorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var cliente = await _clienteService.ObterPorIdAsync(id, cancellationToken);

            if (cliente == null)
                return NotFound();

            var clienteContrato = _conversorCliente.ConverterClienteparaContrato(cliente);
            var viewModel = _conversorCliente.ConverterContratoparaViewModel(clienteContrato);

            return Ok(viewModel);

        }

        [HttpGet]
        public async Task<IActionResult> ObterClientesPorUsuarioId([FromQuery] Guid usuarioId, CancellationToken cancellationToken)
        {
            var cliente = await _clienteService.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);

            if (cliente == null)
                return NotFound();

            var clienteContrato = _conversorCliente.ConverterClienteparaContrato(cliente);
            var viewModels = _conversorCliente.ConverterContratoparaViewModel(clienteContrato);

            return Ok(viewModels);
        }

        [HttpPost]
        public async Task<IActionResult> CriarCliente(ClienteViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorCliente.ConverterViewModelparaContrato(viewmodel);
            var cliente = _conversorCliente.ConverterContratoparaCliente(contrato);

            if (cliente == null)
                return BadRequest();

            var clienteCriado = await _clienteService.CriarAsync(cliente, cancellationToken);
            var clienteContrato = _conversorCliente.ConverterClienteparaContrato(clienteCriado);
            var viewModel = _conversorCliente.ConverterContratoparaViewModel(clienteContrato);

            return Ok(viewModel);
        }

        [HttpPut]
        public async Task<IActionResult> AtualizarCliente(ClienteViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorCliente.ConverterViewModelparaContrato(viewmodel);
            var cliente = _conversorCliente.ConverterContratoparaCliente(contrato);

            if (cliente == null)
                return BadRequest();

            var clienteAtualizado = await _clienteService.AtualizarAsync(cliente, cancellationToken);
            var clienteContrato = _conversorCliente.ConverterClienteparaContrato(clienteAtualizado);
            var viewModel = _conversorCliente.ConverterContratoparaViewModel(clienteContrato);

            return Ok(viewModel);
        }
    }
}
