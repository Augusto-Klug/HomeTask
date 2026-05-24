using System.Security.Claims;
using HomeTask.Application.Dtos;
using HomeTask.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTask.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class AgendamentoController : ControllerBase
    {
        private readonly IAgendamentoService _agendamentoService;
        private readonly IClienteService _clienteService;
        private readonly IPrestadorService _prestadorService;

        public AgendamentoController(
            IAgendamentoService agendamentoService,
            IClienteService clienteService,
            IPrestadorService prestadorService)
        {
            _agendamentoService = agendamentoService;
            _clienteService = clienteService;
            _prestadorService = prestadorService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterAgendamentoPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var agendamento = await _agendamentoService.ObterPorIdAsync(id, cancellationToken);
            if (agendamento == null)
                return NotFound();

            return Ok(agendamento);
        }

        [HttpGet]
        public async Task<IActionResult> ObterAgendamentosPorCliente([FromQuery] Guid clienteId, CancellationToken cancellationToken)
        {
            return Ok(await _agendamentoService.ObterPorClienteAsync(clienteId, cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> ObterAgendamentosPorPrestador([FromQuery] Guid prestadorId, CancellationToken cancellationToken)
        {
            return Ok(await _agendamentoService.ObterPorPrestadorAsync(prestadorId, cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> ObterSolicitacoesPendentesPrestador(CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized("Não foi possível identificar o usuário autenticado.");

            var prestador = await _prestadorService.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);
            if (prestador == null)
                return Forbid("Usuário não possui perfil de prestador.");

            return Ok(await _agendamentoService.ObterSolicitacoesPendentesPorPrestadorAsync(prestador.Id, cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> ObterMeusAgendamentosPrestador(CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized("Não foi possível identificar o usuário autenticado.");

            var prestador = await _prestadorService.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);
            if (prestador == null)
                return Forbid("Usuário não possui perfil de prestador.");

            return Ok(await _agendamentoService.ObterPorPrestadorAsync(prestador.Id, cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> ObterMeusAgendamentosCliente(CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized("Não foi possível identificar o usuário autenticado.");

            var cliente = await _clienteService.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);
            if (cliente == null)
                return Forbid("Usuário não possui perfil de cliente.");

            return Ok(await _agendamentoService.ObterPorClienteAsync(cliente.Id, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> CriarAgendamento(AgendamentoDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _agendamentoService.CriarAsync(dto, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> AceitarAgendamento(AgendamentoDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _agendamentoService.AceitarAsync(dto.Id, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> RecusarAgendamento(AgendamentoDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _agendamentoService.RecusarAsync(dto.Id, dto.MotivoRecusa ?? string.Empty, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> IniciarAgendamento(AgendamentoDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _agendamentoService.IniciarAsync(dto.Id, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> ConcluirAgendamento(AgendamentoDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _agendamentoService.ConcluirAsync(dto.Id, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> CancelarAgendamento(AgendamentoDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _agendamentoService.CancelarAsync(dto.Id, dto.MotivoRecusa ?? string.Empty, cancellationToken));
        }
    }
}
