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
    public class MensagemController : ControllerBase
    {
        private readonly IMensagemService _mensagemService;
        private readonly IAgendamentoService _agendamentoService;
        private readonly IClienteService _clienteService;
        private readonly IPrestadorService _prestadorService;

        public MensagemController(
            IMensagemService mensagemService,
            IAgendamentoService agendamentoService,
            IClienteService clienteService,
            IPrestadorService prestadorService)
        {
            _mensagemService = mensagemService;
            _agendamentoService = agendamentoService;
            _clienteService = clienteService;
            _prestadorService = prestadorService;
        }

        [HttpGet]
        public async Task<ActionResult<MensagemDto>> ObterMensagemPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var mensagem = await _mensagemService.ObterPorIdAsync(id, cancellationToken);
            if (mensagem == null)
                return NotFound();

            return mensagem;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MensagemDto>>> ObterConversa([FromQuery] Guid conversaId, CancellationToken cancellationToken)
        {
            var conversa = await _mensagemService.ObterConversaAsync(conversaId, cancellationToken);
            return conversa.ToList();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MensagemDto>>> ObterPorAgendamento([FromQuery] Guid agendamentoId, CancellationToken cancellationToken)
        {
            var autorizado = await UsuarioPodeAcessarAgendamentoAsync(agendamentoId, cancellationToken);
            if (!autorizado)
                return Forbid();

            var mensagens = await _mensagemService.ObterPorAgendamentoAsync(agendamentoId, cancellationToken);
            return mensagens.ToList();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MensagemDto>>> ObterConversasPorUsuario([FromQuery] Guid usuarioId, CancellationToken cancellationToken)
        {
            var conversas = await _mensagemService.ObterConversasPorUsuarioAsync(usuarioId, cancellationToken);
            return conversas.ToList();
        }

        [HttpGet]
        public async Task<ActionResult<int>> ObterNaoLidas([FromQuery] Guid usuarioId, CancellationToken cancellationToken)
        {
            var total = await _mensagemService.ObterNaoLidasAsync(usuarioId, cancellationToken);

            return total;
        }

        [HttpPost]
        public async Task<ActionResult<MensagemDto>> EnviarMensagem(MensagemDto dto, CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized("Não foi possível identificar o usuário autenticado.");

            if (dto.AgendamentoId == null || !await UsuarioPodeAcessarAgendamentoAsync(dto.AgendamentoId.Value, cancellationToken))
                return Forbid();

            var mensagem = await _mensagemService.EnviarPorAgendamentoAsync(dto.AgendamentoId.Value, usuarioId, dto.Conteudo, cancellationToken);
            return mensagem;
        }

        [HttpPost]
        public async Task<IActionResult> MarcarComoLida(MensagemDto dto, CancellationToken cancellationToken)
        {
            await _mensagemService.MarcarComoLidaAsync(dto.Id, cancellationToken);
            return Ok();
        }

        private async Task<bool> UsuarioPodeAcessarAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return false;

            var agendamento = await _agendamentoService.ObterPorIdAsync(agendamentoId, cancellationToken);
            if (agendamento == null || !StatusPermiteChat(agendamento.Status))
                return false;

            var cliente = await _clienteService.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);
            if (cliente?.Id == agendamento.ClienteId)
                return true;

            var prestador = await _prestadorService.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);
            return prestador?.Id == agendamento.PrestadorId;
        }

        private static bool StatusPermiteChat(StatusAgendamento status) =>
            status is StatusAgendamento.Aceito
                or StatusAgendamento.EmAndamento
                or StatusAgendamento.AguardandoPagamento
                or StatusAgendamento.Concluido;
    }
}
