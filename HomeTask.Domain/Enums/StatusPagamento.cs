namespace HomeTask.Domain.Enums;

/// <summary>
/// Status do pagamento
/// </summary>
public enum StatusPagamento
{
    Pendente = 1,
    Processando = 2,
    Aprovado = 3,
    Recusado = 4,
    Estornado = 5
}
