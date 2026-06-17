namespace HomeTask.Application.Interfaces;

public interface IEmailService
{
    Task EnviarResetSenhaAsync(string destinatario, string nome, string token, int validadeMinutos, CancellationToken cancellationToken = default);
}
