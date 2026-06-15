namespace HomeTask.Infrastructure.Services;

public class MercadoPagoOptions
{
    public const string SectionName = "MercadoPago";

    public string AccessToken { get; set; } = string.Empty;
    public string ApiBaseUrl { get; set; } = string.Empty;
    public string FrontendBaseUrl { get; set; } = string.Empty;
    public string WebhookPath { get; set; } = "/api/Pagamento/WebhookMercadoPago";
    public string ReturnPathTemplate { get; set; } = "/api/Pagamento/RetornoCheckout/{agendamentoId}";
}
