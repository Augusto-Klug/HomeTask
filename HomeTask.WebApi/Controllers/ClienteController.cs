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
        public async Task<ActionResult<ClienteDto>> ObterClientePorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var cliente = await _clienteService.ObterPorIdAsync(id, cancellationToken);
            if (cliente == null)
                return NotFound();

            return cliente;

        }

        [HttpGet]
        public async Task<ActionResult<ClienteDto>> ObterClientesPorUsuarioId([FromQuery] Guid usuarioId, CancellationToken cancellationToken)
        {
            var cliente = await _clienteService.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);
            if (cliente == null)
                return NotFound();

            return cliente;
        }

        [HttpPost]
        public async Task<ActionResult<ClienteDto>> CriarCliente(ClienteDto dto, CancellationToken cancellationToken)
        {
            var cliente = await _clienteService.CriarAsync(dto, cancellationToken);
            return cliente;
        }

        [HttpPut]
        public async Task<ActionResult<ClienteDto>> AtualizarCliente(ClienteDto dto, CancellationToken cancellationToken)
        {
            var cliente = await _clienteService.AtualizarAsync(dto, cancellationToken);
            return cliente;
        }
    }
}
