using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using HomeTask.Application.Dtos;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Enums;
using Microsoft.Extensions.Options;

namespace HomeTask.Infrastructure.Services;

public class MercadoPagoPaymentGateway : IPagamentoGateway
{
    private const string BaseUrl = "https://api.mercadopago.com";
    private readonly HttpClient _httpClient;
    private readonly MercadoPagoOptions _options;

    public MercadoPagoPaymentGateway(HttpClient httpClient, IOptions<MercadoPagoOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<PagamentoCheckoutResponseDto> CriarCheckoutPixAsync(PagamentoCheckoutRequestDto pagamento, CancellationToken cancellationToken = default)
    {
        ValidarConfiguracao();

        var requestBody = new
        {
            items = new[]
            {
                new
                {
                    title = pagamento.Descricao,
                    quantity = 1,
                    currency_id = "BRL",
                    unit_price = pagamento.Valor
                }
            },
            external_reference = pagamento.PagamentoId.ToString(),
            payment_methods = new
            {
                installments = 1
            },
            notification_url = ConstruirUrlApi(_options.WebhookPath),
            back_urls = new
            {
                success = ConstruirUrlApi(ConstruirReturnPath(pagamento.AgendamentoId)),
                pending = ConstruirUrlApi(ConstruirReturnPath(pagamento.AgendamentoId)),
                failure = ConstruirUrlApi(ConstruirReturnPath(pagamento.AgendamentoId))
            },
            auto_return = "approved",
            payer = new
            {
                email = pagamento.ClienteEmail,
                first_name = pagamento.ClienteNome,
                identification = string.IsNullOrWhiteSpace(pagamento.ClienteDocumento)
                    ? null
                    : new
                    {
                        type = "CPF",
                        number = pagamento.ClienteDocumento
                    }
            }
        };
        var requestPayload = JsonSerializer.Serialize(requestBody);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/checkout/preferences");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);
        request.Content = new StringContent(requestPayload, Encoding.UTF8, "application/json");

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorPayload = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Mercado Pago rejeitou a criacao do checkout. Status={(int)response.StatusCode} ({response.StatusCode}). " +
                $"Response={errorPayload}. Request={requestPayload}");
        }

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        using var document = JsonDocument.Parse(payload);
        var root = document.RootElement;

        return new PagamentoCheckoutResponseDto
        {
            CheckoutExternoId = root.GetProperty("id").GetString() ?? string.Empty,
            CheckoutUrl = root.TryGetProperty("init_point", out var initPoint)
                ? initPoint.GetString() ?? string.Empty
                : string.Empty,
            StatusExterno = root.TryGetProperty("collector_id", out var collectorId)
                ? collectorId.ToString()
                : null,
            PayloadExterno = payload
        };
    }

    public async Task<PagamentoStatusGatewayDto?> ObterStatusPagamentoAsync(string pagamentoExternoId, CancellationToken cancellationToken = default)
    {
        ValidarConfiguracao();

        using var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/v1/payments/{pagamentoExternoId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        using var document = JsonDocument.Parse(payload);
        var root = document.RootElement;
        var statusExterno = root.TryGetProperty("status", out var statusElement) ? statusElement.GetString() : null;

        return new PagamentoStatusGatewayDto
        {
            PagamentoExternoId = root.TryGetProperty("id", out var idElement) ? idElement.ToString() : pagamentoExternoId,
            ReferenciaInterna = root.TryGetProperty("external_reference", out var referenceElement)
                ? referenceElement.GetString() ?? string.Empty
                : string.Empty,
            Status = TraduzirStatus(statusExterno),
            StatusExterno = statusExterno,
            MotivoRecusa = root.TryGetProperty("status_detail", out var detailElement) ? detailElement.GetString() : null,
            PayloadExterno = payload
        };
    }

    private void ValidarConfiguracao()
    {
        if (string.IsNullOrWhiteSpace(_options.AccessToken))
            throw new InvalidOperationException("MercadoPago:AccessToken nao configurado.");
        if (string.IsNullOrWhiteSpace(_options.ApiBaseUrl))
            throw new InvalidOperationException("MercadoPago:ApiBaseUrl nao configurado.");
        if (string.IsNullOrWhiteSpace(_options.FrontendBaseUrl))
            throw new InvalidOperationException("MercadoPago:FrontendBaseUrl nao configurado.");
    }

    private string ConstruirUrlApi(string path)
    {
        return $"{_options.ApiBaseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
    }

    private string ConstruirUrlFrontend(string path)
    {
        return $"{_options.FrontendBaseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
    }

    private string ConstruirReturnPath(Guid agendamentoId)
    {
        return _options.ReturnPathTemplate.Replace("{agendamentoId}", agendamentoId.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    private static StatusPagamento TraduzirStatus(string? statusExterno)
    {
        return statusExterno switch
        {
            "approved" => StatusPagamento.Aprovado,
            "rejected" or "cancelled" => StatusPagamento.Recusado,
            "pending" or "in_process" => StatusPagamento.Processando,
            _ => StatusPagamento.Pendente
        };
    }
}
