using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HomeTask.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HomeTask.WebApi.Controller
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IConfiguration _configuration;

        public AuthController(IUsuarioService usuarioService, IConfiguration configuration)
        {
            _usuarioService = usuarioService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
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

            Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true, 
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(double.Parse(_configuration["Jwt:ExpirationHours"]!))
            });


            return Ok(new LoginResponse
            {
                Token = token,
                UserId = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Tipo = (int)usuario.TipoUsuario
            });
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
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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
}
