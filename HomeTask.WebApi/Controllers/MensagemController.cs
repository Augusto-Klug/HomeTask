using HomeTask.Application.Dtos;
using HomeTask.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeTask.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MensagemController : ControllerBase
    {
        private readonly IMensagemService _mensagemService;

        public MensagemController(IMensagemService mensagemService)
        {
            _mensagemService = mensagemService;
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
            var mensagem = await _mensagemService.EnviarAsync(dto, cancellationToken);
            return mensagem;
        }

        [HttpPost]
        public async Task<IActionResult> MarcarComoLida(MensagemDto dto, CancellationToken cancellationToken)
        {
            await _mensagemService.MarcarComoLidaAsync(dto.Id, cancellationToken);
            return Ok();
        }
    }
}
