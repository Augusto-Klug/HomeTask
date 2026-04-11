using HomeTask.Application.Interfaces;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeTask.WebApi.Controller
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MensagemController : ControllerBase
    {
        private readonly IMensagemService _mensagemService;
        private readonly IConversorMensagem _conversorMensagem;

        public MensagemController(IMensagemService mensagemService, IConversorMensagem conversorMensagem)
        {
            _mensagemService = mensagemService;
            _conversorMensagem = conversorMensagem;
        }

        [HttpGet]
        public async Task<IActionResult> ObterMensagemPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var mensagem = await _mensagemService.ObterPorIdAsync(id, cancellationToken);

            if (mensagem == null)
                return NotFound();

            var mensagemContrato = _conversorMensagem.ConverterMensagemparaContrato(mensagem);
            var viewModel = _conversorMensagem.ConverterContratoparaViewModel(mensagemContrato);

            return Ok(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ObterConversa([FromQuery] Guid remetenteId, [FromQuery] Guid destinatarioId, CancellationToken cancellationToken)
        {
            var mensagens = await _mensagemService.ObterConversaAsync(remetenteId, destinatarioId, cancellationToken);

            var viewModels = mensagens.Select(m =>
            {
                var c = _conversorMensagem.ConverterMensagemparaContrato(m);
                return _conversorMensagem.ConverterContratoparaViewModel(c);
            });

            return Ok(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> ObterConversasPorUsuario([FromQuery] Guid usuarioId, CancellationToken cancellationToken)
        {
            var mensagens = await _mensagemService.ObterConversasPorUsuarioAsync(usuarioId, cancellationToken);

            var viewModels = mensagens.Select(m =>
            {
                var c = _conversorMensagem.ConverterMensagemparaContrato(m);
                return _conversorMensagem.ConverterContratoparaViewModel(c);
            });

            return Ok(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> ObterNaoLidas([FromQuery] Guid usuarioId, CancellationToken cancellationToken)
        {
            var total = await _mensagemService.ObterNaoLidasAsync(usuarioId, cancellationToken);

            return Ok(total);
        }

        [HttpPost]
        public async Task<IActionResult> EnviarMensagem(MensagemViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorMensagem.ConverterViewModelparaContrato(viewmodel);
            var mensagem = _conversorMensagem.ConverterContratoparaMensagem(contrato);

            if (mensagem == null)
                return BadRequest();

            var mensagemEnviada = await _mensagemService.EnviarAsync(mensagem, cancellationToken);
            var mensagemContrato = _conversorMensagem.ConverterMensagemparaContrato(mensagemEnviada);
            var viewModel = _conversorMensagem.ConverterContratoparaViewModel(mensagemContrato);

            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> MarcarComoLida(MensagemViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorMensagem.ConverterViewModelparaContrato(viewmodel);
            await _mensagemService.MarcarComoLidaAsync(contrato.Id, cancellationToken);

            return Ok();
        }
    }
}
