using System.Security.Claims;
using HomeTask.Application.Dtos;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Enums;
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
        public async Task<ActionResult<AgendamentoResumoDto>> ObterAgendamentoPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var agendamento = await _agendamentoService.ObterPorIdAsync(id, cancellationToken);
            if (agendamento == null)
                return NotFound();

            return agendamento;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AgendamentoResumoDto>>> ObterAgendamentosPorCliente([FromQuery] Guid clienteId, CancellationToken cancellationToken)
        {
            var agendamentos = await _agendamentoService.ObterPorClienteAsync(clienteId, cancellationToken);
            return agendamentos.ToList();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AgendamentoResumoDto>>> ObterAgendamentosPorPrestador([FromQuery] Guid prestadorId, CancellationToken cancellationToken)
        {
            var agendamentos = await _agendamentoService.ObterPorPrestadorAsync(prestadorId, cancellationToken);
            return agendamentos.ToList();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AgendamentoResumoDto>>> ObterSolicitacoesPendentesPrestador(CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized("Não foi possível identificar o usuário autenticado.");

            var prestador = await _prestadorService.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);
            if (prestador == null)
                return Forbid("Usuário não possui perfil de prestador.");

            var solicitacoes = await _agendamentoService.ObterSolicitacoesPendentesPorPrestadorAsync(prestador.Id, cancellationToken);
            var solicitacoesPrestador = solicitacoes.Where(a => a.AguardandoRespostaDe == TipoUsuario.Prestador).ToList();
            return solicitacoesPrestador;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AgendamentoResumoDto>>> ObterMeusAgendamentosPrestador(CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized("Não foi possível identificar o usuário autenticado.");

            var prestador = await _prestadorService.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);
            if (prestador == null)
                return Forbid("Usuário não possui perfil de prestador.");

            var agendamentos = await _agendamentoService.ObterPorPrestadorAsync(prestador.Id, cancellationToken);
            return agendamentos.ToList();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AgendamentoResumoDto>>> ObterMeusAgendamentosCliente(CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized("Não foi possível identificar o usuário autenticado.");

            var cliente = await _clienteService.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);
            if (cliente == null)
                return Forbid("Usuário não possui perfil de cliente.");

            var agendamentos = await _agendamentoService.ObterPorClienteAsync(cliente.Id, cancellationToken);
            return agendamentos.ToList();
        }

        [HttpPost]
        public async Task<ActionResult<AgendamentoDto>> CriarAgendamento(AgendamentoDto dto, CancellationToken cancellationToken)
        {
            var agendamento = await _agendamentoService.CriarAsync(dto, cancellationToken);
            return agendamento;
        }

        [HttpPost]
        public async Task<ActionResult<AgendamentoDto>> AceitarAgendamento(AgendamentoDto dto, CancellationToken cancellationToken)
        {
            var agendamento = await _agendamentoService.AceitarAsync(dto.Id, cancellationToken);
            return agendamento;
        }

        [HttpPost]
        public async Task<ActionResult<AgendamentoDto>> RecusarAgendamento(AgendamentoDto dto, CancellationToken cancellationToken)
        {
            var agendamento = await _agendamentoService.RecusarAsync(dto.Id, dto.MotivoRecusa ?? string.Empty, cancellationToken);
            return agendamento;
        }

        [HttpPost]
        public async Task<ActionResult<AgendamentoDto>> IniciarAgendamento(AgendamentoDto dto, CancellationToken cancellationToken)
        {
            var agendamento = await _agendamentoService.IniciarAsync(dto.Id, cancellationToken);
            return agendamento;
        }

        [HttpPost]
        public async Task<ActionResult<AgendamentoDto>> ConcluirAgendamento(AgendamentoDto dto, CancellationToken cancellationToken)
        {
            var agendamento = await _agendamentoService.ConcluirAsync(dto.Id, cancellationToken);
            return agendamento;
        }

        [HttpPost]
        public async Task<ActionResult<AgendamentoDto>> CancelarAgendamento(AgendamentoDto dto, CancellationToken cancellationToken)
        {
            var agendamento = await _agendamentoService.CancelarAsync(dto.Id, dto.MotivoRecusa ?? string.Empty, cancellationToken);
            return agendamento;
        }
    }
}
