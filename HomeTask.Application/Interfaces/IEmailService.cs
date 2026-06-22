namespace HomeTask.Application.Interfaces;

public interface IEmailService
{
    Task EnviarResetSenhaAsync(string destinatario, string nome, string token, int validadeMinutos, CancellationToken cancellationToken = default);
    Task EnviarAvisoBaixaAvaliacaoPrestadorAsync(string destinatario, string nome, decimal mediaAvaliacoes, int totalAvaliacoes, CancellationToken cancellationToken = default);
    Task EnviarAvisoSuspensaoPrestadorAsync(string destinatario, string nome, DateTime dataFimSuspensao, CancellationToken cancellationToken = default);
}
