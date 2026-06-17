namespace HomeTask.Application.Interfaces;

public interface IAuthService
{
    Task SolicitarResetSenhaAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> RedefinirSenhaAsync(string token, string novaSenha, CancellationToken cancellationToken = default);
}
