namespace HomeTask.Infrastructure.Services;

public class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public bool UseOAuth2 { get; set; }
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = "HomeTask";
    public string FrontendBaseUrl { get; set; } = "http://localhost:8080";
    public string ResetPasswordPath { get; set; } = "/redefinir-senha";
}
