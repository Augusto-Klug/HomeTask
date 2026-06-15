using HomeTask.Application.Dtos;
using HomeTask.Application.Interfaces;
using System.Security.Claims;
using System.Text.Json;
using HomeTask.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HomeTask.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PagamentoController : ControllerBase
    {
        private readonly IPagamentoService _pagamentoService;
        private readonly IClienteService _clienteService;
        private readonly MercadoPagoOptions _mercadoPagoOptions;

        public PagamentoController(
            IPagamentoService pagamentoService,
            IClienteService clienteService,
            IOptions<MercadoPagoOptions> mercadoPagoOptions)
        {
            _pagamentoService = pagamentoService;
            _clienteService = clienteService;
            _mercadoPagoOptions = mercadoPagoOptions.Value;
        }

        [HttpGet]
        public async Task<ActionResult<PagamentoDto>> ObterPagamentoPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var pagamento = await _pagamentoService.ObterPorIdAsync(id, cancellationToken);
            if (pagamento == null)
                return NotFound();

            return pagamento;
        }

        [HttpGet]
        public async Task<ActionResult<PagamentoDto>> ObterPagamentoPorAgendamento([FromQuery] Guid agendamentoId, CancellationToken cancellationToken)
        {
            var pagamento = await _pagamentoService.ObterPorAgendamentoAsync(agendamentoId, cancellationToken);
            if (pagamento == null)
                return NotFound();

            return pagamento;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PagamentoDto>> IniciarPagamento(IniciarPagamentoDto dto, CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized("Nao foi possivel identificar o usuario autenticado.");

            var cliente = await _clienteService.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);
            if (cliente == null)
                return StatusCode(StatusCodes.Status403Forbidden, "Usuario nao possui perfil de cliente.");

            var pagamento = await _pagamentoService.IniciarCheckoutAsync(dto.AgendamentoId, cliente.Id, cancellationToken);
            return pagamento;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PagamentoDto>> ReconciliarPagamento(ReconciliarPagamentoDto dto, CancellationToken cancellationToken)
        {
            var pagamento = await _pagamentoService.ReconciliarPagamentoExternoAsync(dto.PagamentoExternoId, cancellationToken);
            if (pagamento == null)
                return NotFound();

            return pagamento;
        }

        [HttpGet("/api/Pagamento/RetornoCheckout/{agendamentoId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> RetornoCheckout(Guid agendamentoId, CancellationToken cancellationToken)
        {
            var pagamentoExternoId = Request.Query["payment_id"].FirstOrDefault()
                ?? Request.Query["collection_id"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(pagamentoExternoId))
            {
                await _pagamentoService.ReconciliarPagamentoExternoAsync(pagamentoExternoId, cancellationToken);
            }

            var query = new Dictionary<string, string?>();
            foreach (var item in Request.Query)
            {
                query[item.Key] = item.Value.ToString();
            }

            query["retornoCheckout"] = "1";

            var queryString = QueryString.Create(query);
            var destino = $"{_mercadoPagoOptions.FrontendBaseUrl.TrimEnd('/')}/agendamento/detalhes/{agendamentoId}{queryString}";
            return Redirect(destino);
        }

        [Route("/api/Pagamento/WebhookMercadoPago")]
        [AcceptVerbs("GET", "POST")]
        [AllowAnonymous]
        public async Task<IActionResult> WebhookMercadoPago(CancellationToken cancellationToken)
        {
            string? payloadExterno = null;
            string? topico = Request.Query["type"].FirstOrDefault() ?? Request.Query["topic"].FirstOrDefault();
            string? acao = Request.Query["action"].FirstOrDefault();
            string? pagamentoExternoId = Request.Query["data.id"].FirstOrDefault()
                ?? Request.Query["id"].FirstOrDefault();

            if (Request.ContentLength > 0)
            {
                using var document = await JsonDocument.ParseAsync(Request.Body, cancellationToken: cancellationToken);
                payloadExterno = document.RootElement.GetRawText();

                if (string.IsNullOrWhiteSpace(topico) &&
                    document.RootElement.TryGetProperty("type", out var typeElement))
                {
                    topico = typeElement.GetString();
                }

                if (string.IsNullOrWhiteSpace(acao) &&
                    document.RootElement.TryGetProperty("action", out var actionElement))
                {
                    acao = actionElement.GetString();
                }

                if (string.IsNullOrWhiteSpace(pagamentoExternoId))
                {
                    if (document.RootElement.TryGetProperty("data", out var dataElement) &&
                        dataElement.TryGetProperty("id", out var paymentIdElement))
                    {
                        pagamentoExternoId = paymentIdElement.ToString();
                    }
                    else if (document.RootElement.TryGetProperty("id", out var idElement))
                    {
                        pagamentoExternoId = idElement.ToString();
                    }
                }
            }

            await _pagamentoService.ProcessarWebhookAsync(
                new PagamentoWebhookDto
                {
                    Topico = topico,
                    Acao = acao,
                    PagamentoExternoId = pagamentoExternoId,
                    PayloadExterno = payloadExterno
                },
                cancellationToken);

            return Ok();
        }
    }
}
