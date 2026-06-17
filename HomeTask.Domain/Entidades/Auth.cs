namespace HomeTask.Domain.Entidades;

public class Auth
{
    public Auth() { }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UsuarioId { get; private set; }
    public string ResetarSenhaToken { get; private set; } = string.Empty;
    public DateTime ResetarSenhaTokenExpiraEm { get; private set; }

    public Usuario Usuario { get; private set; } = null!;

    public bool ResetarSenhaTokenValido(string token, DateTime agoraUtc) =>
        ResetarSenhaToken == token && ResetarSenhaTokenExpiraEm >= agoraUtc;

    public void DefinirResetarSenhaToken(Guid usuarioId, string token)
    {
        UsuarioId = usuarioId;
        ResetarSenhaToken = token;
        ResetarSenhaTokenExpiraEm = DateTime.UtcNow.AddMinutes(15);
    }

    public void InvalidarResetarSenhaToken()
    {
        ResetarSenhaToken = $"usado-{Guid.NewGuid():N}";
        ResetarSenhaTokenExpiraEm = DateTime.UtcNow;
    }
}
