using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Entidades;

/// <summary>
/// Pagamento de serviço contratado (RF05, NEG04)
/// </summary>
public class Pagamento
{
    public Pagamento() { }

    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid AgendamentoId { get; private set; }

    public Agendamento Agendamento { get; private set; } = null!;

    /// <summary>
    /// Valor do pagamento
    /// </summary>
    public decimal Valor { get; private set; }

    /// <summary>
    /// Tipo de pagamento (Pix, Débito, Crédito)
    /// </summary>
    public TipoPagamento TipoPagamento { get; private set; }

    /// <summary>
    /// Status do pagamento
    /// </summary>
    public StatusPagamento Status { get; private set; } = StatusPagamento.Pendente;

    /// <summary>
    /// Identificador da transação no gateway de pagamento
    /// </summary>
    public string? TransacaoId { get; private set; }

    public string? CheckoutExternoId { get; private set; }

    public string? CheckoutUrl { get; private set; }

    public string? StatusExterno { get; private set; }

    public string? PayloadExterno { get; private set; }

    public DateTime DataCriacao { get; private set; } = DateTime.UtcNow;

    public DateTime? DataProcessamento { get; private set; }

    public DateTime? DataConfirmacao { get; private set; }

    public string? MotivoRecusa { get; private set; }

    public void DefinirDados(
        Guid id,
        Guid agendamentoId,
        decimal valor,
        TipoPagamento tipoPagamento,
        StatusPagamento status,
        string? transacaoId,
        string? checkoutExternoId,
        string? checkoutUrl,
        string? statusExterno,
        string? payloadExterno,
        DateTime dataCriacao,
        DateTime? dataProcessamento,
        DateTime? dataConfirmacao,
        string? motivoRecusa)
    {
        Id = id;
        AgendamentoId = agendamentoId;
        Valor = valor;
        TipoPagamento = tipoPagamento;
        Status = status;
        TransacaoId = transacaoId;
        CheckoutExternoId = checkoutExternoId;
        CheckoutUrl = checkoutUrl;
        StatusExterno = statusExterno;
        PayloadExterno = payloadExterno;
        DataCriacao = dataCriacao;
        DataProcessamento = dataProcessamento;
        DataConfirmacao = dataConfirmacao;
        MotivoRecusa = motivoRecusa;
    }

    public void DefinirComoPendente(DateTime dataCriacao)
    {
        Status = StatusPagamento.Pendente;
        DataCriacao = dataCriacao;
    }

    public void Processar(DateTime dataProcessamento)
    {
        Status = StatusPagamento.Processando;
        DataProcessamento = dataProcessamento;
    }

    public void RegistrarCheckout(string checkoutExternoId, string checkoutUrl, string? statusExterno, string? payloadExterno)
    {
        CheckoutExternoId = checkoutExternoId;
        CheckoutUrl = checkoutUrl;
        StatusExterno = statusExterno;
        PayloadExterno = payloadExterno;
    }

    public void AtualizarRetornoGateway(string? transacaoId, string? statusExterno, string? payloadExterno, string? motivoRecusa)
    {
        if (!string.IsNullOrWhiteSpace(transacaoId))
            TransacaoId = transacaoId;

        StatusExterno = statusExterno;
        PayloadExterno = payloadExterno;
        MotivoRecusa = motivoRecusa;
    }

    public void Aprovar(string transacaoId, DateTime dataConfirmacao)
    {
        Status = StatusPagamento.Aprovado;
        TransacaoId = transacaoId;
        DataConfirmacao = dataConfirmacao;
    }

    public void Recusar(string motivo)
    {
        Status = StatusPagamento.Recusado;
        MotivoRecusa = motivo;
    }

    public void Estornar()
    {
        Status = StatusPagamento.Estornado;
    }
}
