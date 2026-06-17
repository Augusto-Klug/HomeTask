using System.Security.Cryptography;
using System.Text;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HomeTask.Application.Services;

public class AuthService : IAuthService
{
    private const int ResetSenhaValidadeMinutos = 15;

    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IAuthRepository authRepository,
        IEmailService emailService,
        ILogger<AuthService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _authRepository = authRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task SolicitarResetSenhaAsync(string email, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando solicitacao de reset de senha para o e-mail {Email}.", email);

        var usuario = await _usuarioRepository.ObterPorEmailAsync(email, cancellationToken);
        if (usuario == null)
        {
            _logger.LogWarning("Solicitacao de reset ignorada para e-mail nao encontrado: {Email}.", email);
            return;
        }

        var token = GerarToken();
        var auth = await _authRepository.ObterPorUsuarioIdAsync(usuario.Id, cancellationToken);
        var novoAuth = auth == null;
        auth ??= new Auth();
        auth.DefinirResetarSenhaToken(usuario.Id, token);

        _logger.LogInformation(
            "Token de reset gerado para usuario {UsuarioId}. Novo registro Auth: {NovoAuth}.",
            usuario.Id,
            novoAuth);

        if (novoAuth)
            await _authRepository.AdicionarAsync(auth, cancellationToken);
        else
            _authRepository.Atualizar(auth);

        await _authRepository.SalvarAlteracoesAsync(cancellationToken);

        _logger.LogInformation("Token de reset persistido para usuario {UsuarioId}.", usuario.Id);

        try
        {
            await _emailService.EnviarResetSenhaAsync(
                usuario.Email,
                usuario.Nome,
                token,
                ResetSenhaValidadeMinutos,
                cancellationToken);

            _logger.LogInformation("E-mail de reset enviado para usuario {UsuarioId}.", usuario.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Falha ao enviar e-mail de reset para usuario {UsuarioId}. Mantendo resposta neutra.",
                usuario.Id);

            // Mantem resposta neutra para nao revelar se o e-mail existe na base.
        }
    }

    public async Task<bool> RedefinirSenhaAsync(string token, string novaSenha, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando redefinicao de senha por token.");

        var auth = await _authRepository.ObterPorResetarSenhaTokenAsync(token, cancellationToken);
        if (auth == null || !auth.ResetarSenhaTokenValido(token, DateTime.UtcNow))
        {
            _logger.LogWarning("Falha na redefinicao de senha: token invalido ou expirado.");
            return false;
        }

        auth.Usuario.DefinirSenhaHash(HashSenha(novaSenha));
        auth.InvalidarResetarSenhaToken();

        _authRepository.Atualizar(auth);
        await _authRepository.SalvarAlteracoesAsync(cancellationToken);

        _logger.LogInformation("Senha redefinida com sucesso para usuario {UsuarioId}.", auth.UsuarioId);

        return true;
    }

    private static string GerarToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-", StringComparison.Ordinal)
            .Replace("/", "_", StringComparison.Ordinal)
            .TrimEnd('=');
    }

    private static string HashSenha(string senha)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
        return Convert.ToBase64String(bytes);
    }

}
