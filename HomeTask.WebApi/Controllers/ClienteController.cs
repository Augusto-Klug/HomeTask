using HomeTask.Application.Dtos;
using HomeTask.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeTask.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterClientePorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var cliente = await _clienteService.ObterPorIdAsync(id, cancellationToken);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);

        }

        [HttpGet]
        public async Task<IActionResult> ObterClientesPorUsuarioId([FromQuery] Guid usuarioId, CancellationToken cancellationToken)
        {
            var cliente = await _clienteService.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> CriarCliente(ClienteDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _clienteService.CriarAsync(dto, cancellationToken));
        }

        [HttpPut]
        public async Task<IActionResult> AtualizarCliente(ClienteDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _clienteService.AtualizarAsync(dto, cancellationToken));
        }
    }
}
