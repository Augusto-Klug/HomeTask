using System.Security.Claims;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces;
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
        private readonly IConversorAgendamento _conversorAgendamento;

        public AgendamentoController(
            IAgendamentoService agendamentoService,
            IClienteService clienteService,
            IPrestadorService prestadorService,
            IConversorAgendamento conversorAgendamento)
        {
            _agendamentoService = agendamentoService;
            _clienteService = clienteService;
            _prestadorService = prestadorService;
            _conversorAgendamento = conversorAgendamento;
        }

        [HttpGet]
        public async Task<IActionResult> ObterAgendamentoPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var agendamento = await _agendamentoService.ObterPorIdAsync(id, cancellationToken);

            if (agendamento == null)
                return NotFound();

            var agendamentoContrato = _conversorAgendamento.ConverterAgendamentoparaResumoContrato(agendamento);
            var viewModel = _conversorAgendamento.ConverterResumoContratoparaViewModel(agendamentoContrato);

            return Ok(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ObterAgendamentosPorCliente([FromQuery] Guid clienteId, CancellationToken cancellationToken)
        {
            var agendamentos = await _agendamentoService.ObterPorClienteAsync(clienteId, cancellationToken);

            var viewModels = agendamentos.Select(a =>
            {
                var c = _conversorAgendamento.ConverterAgendamentoparaContrato(a);
                return _conversorAgendamento.ConverterContratoparaViewModel(c);
            });

            return Ok(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> ObterAgendamentosPorPrestador([FromQuery] Guid prestadorId, CancellationToken cancellationToken)
        {
            var agendamentos = await _agendamentoService.ObterPorPrestadorAsync(prestadorId, cancellationToken);

            var viewModels = agendamentos.Select(a =>
            {
                var c = _conversorAgendamento.ConverterAgendamentoparaContrato(a);
                return _conversorAgendamento.ConverterContratoparaViewModel(c);
            });

            return Ok(viewModels);
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

            var agendamentos = await _agendamentoService.ObterSolicitacoesPendentesPorPrestadorAsync(prestador.Id, cancellationToken);
            var viewModels = agendamentos.Select(a =>
            {
                var contrato = _conversorAgendamento.ConverterAgendamentoparaResumoContrato(a);
                return _conversorAgendamento.ConverterResumoContratoparaViewModel(contrato);
            });

            return Ok(viewModels);
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

            var agendamentos = await _agendamentoService.ObterPorPrestadorAsync(prestador.Id, cancellationToken);
            var viewModels = agendamentos.Select(a =>
            {
                var contrato = _conversorAgendamento.ConverterAgendamentoparaResumoContrato(a);
                return _conversorAgendamento.ConverterResumoContratoparaViewModel(contrato);
            });

            return Ok(viewModels);
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

            var agendamentos = await _agendamentoService.ObterPorClienteAsync(cliente.Id, cancellationToken);
            var viewModels = agendamentos.Select(a =>
            {
                var contrato = _conversorAgendamento.ConverterAgendamentoparaResumoContrato(a);
                return _conversorAgendamento.ConverterResumoContratoparaViewModel(contrato);
            });

            return Ok(viewModels);
        }

        [HttpPost]
        public async Task<IActionResult> CriarAgendamento(AgendamentoViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorAgendamento.ConverterViewModelparaContrato(viewmodel);
            var agendamento = _conversorAgendamento.ConverterContratoparaAgendamento(contrato);

            if (agendamento == null)
                return BadRequest();

            var agendamentoCriado = await _agendamentoService.CriarAsync(agendamento, contrato.ServicosOferecidosIds, cancellationToken);
            var agendamentoContrato = _conversorAgendamento.ConverterAgendamentoparaContrato(agendamentoCriado);
            var viewModel = _conversorAgendamento.ConverterContratoparaViewModel(agendamentoContrato);

            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AceitarAgendamento(AgendamentoViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorAgendamento.ConverterViewModelparaContrato(viewmodel);
            var agendamento = await _agendamentoService.AceitarAsync(contrato.Id, cancellationToken);
            var agendamentoContrato = _conversorAgendamento.ConverterAgendamentoparaContrato(agendamento);
            var viewModel = _conversorAgendamento.ConverterContratoparaViewModel(agendamentoContrato);

            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RecusarAgendamento(AgendamentoViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorAgendamento.ConverterViewModelparaContrato(viewmodel);
            var agendamento = await _agendamentoService.RecusarAsync(contrato.Id, contrato.MotivoRecusa ?? string.Empty, cancellationToken);
            var agendamentoContrato = _conversorAgendamento.ConverterAgendamentoparaContrato(agendamento);
            var viewModel = _conversorAgendamento.ConverterContratoparaViewModel(agendamentoContrato);

            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> IniciarAgendamento(AgendamentoViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorAgendamento.ConverterViewModelparaContrato(viewmodel);
            var agendamento = await _agendamentoService.IniciarAsync(contrato.Id, cancellationToken);
            var agendamentoContrato = _conversorAgendamento.ConverterAgendamentoparaContrato(agendamento);
            var viewModel = _conversorAgendamento.ConverterContratoparaViewModel(agendamentoContrato);

            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConcluirAgendamento(AgendamentoViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorAgendamento.ConverterViewModelparaContrato(viewmodel);
            var agendamento = await _agendamentoService.ConcluirAsync(contrato.Id, cancellationToken);
            var agendamentoContrato = _conversorAgendamento.ConverterAgendamentoparaContrato(agendamento);
            var viewModel = _conversorAgendamento.ConverterContratoparaViewModel(agendamentoContrato);

            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CancelarAgendamento(AgendamentoViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorAgendamento.ConverterViewModelparaContrato(viewmodel);
            var agendamento = await _agendamentoService.CancelarAsync(contrato.Id, contrato.MotivoRecusa ?? string.Empty, cancellationToken);
            var agendamentoContrato = _conversorAgendamento.ConverterAgendamentoparaContrato(agendamento);
            var viewModel = _conversorAgendamento.ConverterContratoparaViewModel(agendamentoContrato);

            return Ok(viewModel);
        }

    }
}
