using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HomeTask.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HomeTask.WebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private const string AccessTokenCookieName = "access_token";

    private readonly IUsuarioService _usuarioService;
    private readonly IConfiguration _configuration;

    public AuthController(IUsuarioService usuarioService, IConfiguration configuration)
    {
        _usuarioService = usuarioService;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
            return BadRequest("E-mail e senha são obrigatórios.");

        var senhaValida = await _usuarioService.ValidarSenhaAsync(request.Email, request.Senha, cancellationToken);
        if (!senhaValida)
            return Unauthorized("E-mail ou senha inválidos.");

        var usuario = await _usuarioService.ObterPorEmailAsync(request.Email, cancellationToken);
        if (usuario == null)
            return Unauthorized("Usuário não encontrado.");

        var token = GerarToken(usuario.Id, usuario.Email, usuario.Nome, (int)usuario.TipoUsuario);
        DefinirCookieAutenticacao(token);

        return CriarRespostaLogin(usuario.Id, usuario.Email, usuario.Nome, (int)usuario.TipoUsuario);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<LoginResponse>> Me(CancellationToken cancellationToken)
    {
        var usuarioId = ObterUsuarioId(User);
        if (usuarioId == null)
            return Unauthorized();

        var usuario = await _usuarioService.ObterPorIdAsync(usuarioId.Value, cancellationToken);
        if (usuario == null)
            return Unauthorized();

        return CriarRespostaLogin(usuario.Id, usuario.Email, usuario.Nome, (int)usuario.TipoUsuario);
    }

    [HttpPost]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(AccessTokenCookieName, CriarOpcoesCookie());
        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<LoginResponse>> Refresh(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(AccessTokenCookieName, out var token) || string.IsNullOrWhiteSpace(token))
            return Unauthorized();

        var principal = ValidarTokenIgnorandoExpiracao(token);
        if (principal == null)
            return Unauthorized();

        var usuarioId = ObterUsuarioId(principal);
        if (usuarioId == null)
            return Unauthorized();

        var usuario = await _usuarioService.ObterPorIdAsync(usuarioId.Value, cancellationToken);
        if (usuario == null)
            return Unauthorized();

        var novoToken = GerarToken(usuario.Id, usuario.Email, usuario.Nome, (int)usuario.TipoUsuario);
        DefinirCookieAutenticacao(novoToken);

        return CriarRespostaLogin(usuario.Id, usuario.Email, usuario.Nome, (int)usuario.TipoUsuario);
    }

    private string GerarToken(Guid userId, string email, string nome, int tipo)
    {
        var jwtConfig = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(double.Parse(jwtConfig["ExpirationHours"]!));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("nome", nome),
            new Claim("tipo", tipo.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtConfig["Issuer"],
            audience: jwtConfig["Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private void DefinirCookieAutenticacao(string token)
    {
        Response.Cookies.Append(AccessTokenCookieName, token, CriarOpcoesCookie());
    }

    private CookieOptions CriarOpcoesCookie()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Path = "/",
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddHours(double.Parse(_configuration["Jwt:ExpirationHours"]!))
        };
    }

    private static Guid? ObterUsuarioId(ClaimsPrincipal principal)
    {
        var userIdClaim =
            principal.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
            principal.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private ClaimsPrincipal? ValidarTokenIgnorandoExpiracao(string token)
    {
        var jwtConfig = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!));
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            return tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig["Issuer"],
                ValidAudience = jwtConfig["Audience"],
                IssuerSigningKey = key
            }, out _);
        }
        catch
        {
            return null;
        }
    }

    private static LoginResponse CriarRespostaLogin(Guid userId, string email, string nome, int tipo)
    {
        return new LoginResponse
        {
            UserId = userId,
            Nome = nome,
            Email = email,
            Tipo = tipo
        };
    }
}

public record LoginRequest(string Email, string Senha);

public record LoginResponse
{
    public string Token { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public int Tipo { get; init; }
}
