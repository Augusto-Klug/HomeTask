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
        public async Task<IActionResult> ObterMensagemPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var mensagem = await _mensagemService.ObterPorIdAsync(id, cancellationToken);
            if (mensagem == null)
                return NotFound();

            return Ok(mensagem);
        }

        [HttpGet]
        public async Task<IActionResult> ObterConversa([FromQuery] Guid conversaId, CancellationToken cancellationToken)
        {
            return Ok(await _mensagemService.ObterConversaAsync(conversaId, cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> ObterConversasPorUsuario([FromQuery] Guid usuarioId, CancellationToken cancellationToken)
        {
            return Ok(await _mensagemService.ObterConversasPorUsuarioAsync(usuarioId, cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> ObterNaoLidas([FromQuery] Guid usuarioId, CancellationToken cancellationToken)
        {
            var total = await _mensagemService.ObterNaoLidasAsync(usuarioId, cancellationToken);

            return Ok(total);
        }

        [HttpPost]
        public async Task<IActionResult> EnviarMensagem(MensagemDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _mensagemService.EnviarAsync(dto, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> MarcarComoLida(MensagemDto dto, CancellationToken cancellationToken)
        {
            await _mensagemService.MarcarComoLidaAsync(dto.Id, cancellationToken);
            return Ok();
        }
    }
}
