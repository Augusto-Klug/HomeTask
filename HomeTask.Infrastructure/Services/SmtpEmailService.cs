using System.Net;
using HomeTask.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HomeTask.Infrastructure.Services;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpOptions _options;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<SmtpOptions> options, ILogger<SmtpEmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task EnviarResetSenhaAsync(string destinatario, string nome, string token, int validadeMinutos, CancellationToken cancellationToken = default)
    {
        ValidarConfiguracao();

        var link = CriarLinkResetSenha(token);
        var corpo = $$"""
        <p>Ola, {{WebUtility.HtmlEncode(nome)}}.</p>
        <p>Recebemos uma solicitacao para redefinir sua senha na HomeTask.</p>
        <p><a href="{{WebUtility.HtmlEncode(link)}}">Clique aqui para redefinir sua senha</a></p>
        <p>Este link expira em {{validadeMinutos}} minutos.</p>
        <p>Se voce nao solicitou esta alteracao, ignore este e-mail.</p>
        """;

        var mensagem = new MimeMessage();
        mensagem.From.Add(new MailboxAddress(_options.FromName, ObterRemetente()));
        mensagem.To.Add(MailboxAddress.Parse(destinatario));
        mensagem.Subject = "Recuperacao de senha HomeTask";
        mensagem.Body = new BodyBuilder { HtmlBody = corpo }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.Host, _options.Port, ObterSecureSocketOptions(), cancellationToken);
        await AutenticarAsync(client, cancellationToken);
        await client.SendAsync(mensagem, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }

    private string CriarLinkResetSenha(string token)
    {
        var baseUrl = _options.FrontendBaseUrl.TrimEnd('/');
        var path = _options.ResetPasswordPath.Trim('/');
        return $"{baseUrl}/{path}/{Uri.EscapeDataString(token)}";
    }

    private string ObterRemetente() =>
        string.IsNullOrWhiteSpace(_options.FromEmail) ? _options.UserName : _options.FromEmail;

    private void ValidarConfiguracao()
    {
        if (string.IsNullOrWhiteSpace(_options.UserName))
            throw new InvalidOperationException("Smtp:UserName nao configurado.");

        if (_options.UseOAuth2)
        {
            if (string.IsNullOrWhiteSpace(_options.AccessToken))
                throw new InvalidOperationException("Smtp:AccessToken nao configurado.");

            return;
        }

        if (string.IsNullOrWhiteSpace(_options.Password) && string.IsNullOrWhiteSpace(_options.AccessToken))
            throw new InvalidOperationException("Smtp:Password nao configurado.");
    }

    private async Task AutenticarAsync(SmtpClient client, CancellationToken cancellationToken)
    {
        if (_options.UseOAuth2)
        {
            _logger.LogInformation("Autenticando SMTP com OAuth2 para {UserName}.", _options.UserName);
            await client.AuthenticateAsync(new SaslMechanismOAuth2(_options.UserName, _options.AccessToken), cancellationToken);
            return;
        }

        var password = _options.Password;
        if (string.IsNullOrWhiteSpace(password))
        {
            password = _options.AccessToken;
            _logger.LogWarning("Smtp:AccessToken esta sendo usado como fallback para senha SMTP. Prefira Smtp:Password para senha de app.");
        }

        _logger.LogInformation("Autenticando SMTP com usuario e senha de app para {UserName}.", _options.UserName);
        await client.AuthenticateAsync(_options.UserName, password, cancellationToken);
    }

    private SecureSocketOptions ObterSecureSocketOptions()
    {
        if (!_options.EnableSsl)
            return SecureSocketOptions.None;

        return _options.Port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
    }
}
