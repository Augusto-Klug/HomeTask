using HomeTask.Application.Services;
using HomeTask.Domain.Entidades;
using HomeTask.Tests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace HomeTask.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task SolicitarResetSenhaAsync_QuandoEmailNaoExiste_DeveManterRespostaNeutraENaoPersistir()
    {
        // Arrange
        var usuarioRepository = new FakeUsuarioRepository();
        var authRepository = new FakeAuthRepository();
        var emailService = new FakeEmailService();
        var service = new AuthService(usuarioRepository, authRepository, emailService, NullLogger<AuthService>.Instance);

        // Act
        await service.SolicitarResetSenhaAsync("ausente@teste.com");

        // Assert
        Assert.Equal(0, authRepository.AdicionarChamadas);
        Assert.Equal(0, authRepository.SalvarChamadas);
        Assert.Equal(0, emailService.ResetSenhaChamadas);
    }

    [Fact]
    public async Task SolicitarResetSenhaAsync_QuandoUsuarioExiste_DevePersistirTokenEEnviarEmail()
    {
        // Arrange
        var usuarioRepository = new FakeUsuarioRepository();
        var authRepository = new FakeAuthRepository();
        var emailService = new FakeEmailService();
        var usuario = EntidadeFactory.CriarUsuario(nome: "Ana", documento: "12345678900");
        usuarioRepository.Seed(usuario);
        var service = new AuthService(usuarioRepository, authRepository, emailService, NullLogger<AuthService>.Instance);

        // Act
        await service.SolicitarResetSenhaAsync(usuario.Email);
        var auth = Assert.IsType<Auth>(authRepository.UltimoAdicionado);

        // Assert
        Assert.Equal(usuario.Id, auth.UsuarioId);
        Assert.False(string.IsNullOrWhiteSpace(auth.ResetarSenhaToken));
        Assert.True(auth.ResetarSenhaTokenExpiraEm > DateTime.UtcNow);
        Assert.Equal(usuario.Email, emailService.UltimoEmail);
        Assert.Equal(auth.ResetarSenhaToken, emailService.UltimoToken);
        Assert.Equal(1, authRepository.AdicionarChamadas);
        Assert.Equal(1, authRepository.SalvarChamadas);
    }

    [Fact]
    public async Task SolicitarResetSenhaAsync_QuandoEmailFalha_DeveManterTokenPersistido()
    {
        // Arrange
        var usuarioRepository = new FakeUsuarioRepository();
        var authRepository = new FakeAuthRepository();
        var emailService = new FakeEmailService { FalharEnvio = true };
        var usuario = EntidadeFactory.CriarUsuario();
        usuarioRepository.Seed(usuario);
        var service = new AuthService(usuarioRepository, authRepository, emailService, NullLogger<AuthService>.Instance);

        // Act
        await service.SolicitarResetSenhaAsync(usuario.Email);

        // Assert
        Assert.Equal(1, authRepository.AdicionarChamadas);
        Assert.Equal(1, authRepository.SalvarChamadas);
        Assert.Equal(1, emailService.ResetSenhaChamadas);
    }

    [Fact]
    public async Task RedefinirSenhaAsync_QuandoTokenValido_DeveAtualizarSenhaEInvalidarToken()
    {
        // Arrange
        var usuarioRepository = new FakeUsuarioRepository();
        var authRepository = new FakeAuthRepository();
        var emailService = new FakeEmailService();
        var usuario = EntidadeFactory.CriarUsuario();
        var auth = new Auth();
        auth.DefinirResetarSenhaToken(usuario.Id, "token-valido");
        EntidadeFactory.DefinirNavegacao(auth, nameof(Auth.Usuario), usuario);
        authRepository.Seed(auth);
        var service = new AuthService(usuarioRepository, authRepository, emailService, NullLogger<AuthService>.Instance);

        // Act
        var redefinido = await service.RedefinirSenhaAsync("token-valido", "nova-senha");

        // Assert
        Assert.True(redefinido);
        Assert.NotEqual("hash", usuario.SenhaHash);
        Assert.False(auth.ResetarSenhaTokenValido("token-valido", DateTime.UtcNow));
        Assert.Equal(1, authRepository.AtualizarChamadas);
        Assert.Equal(1, authRepository.SalvarChamadas);
    }

    [Fact]
    public async Task RedefinirSenhaAsync_QuandoTokenInvalido_DeveRetornarFalseENaoSalvar()
    {
        // Arrange
        var service = new AuthService(new FakeUsuarioRepository(), new FakeAuthRepository(), new FakeEmailService(), NullLogger<AuthService>.Instance);

        // Act
        var redefinido = await service.RedefinirSenhaAsync("token-inexistente", "nova-senha");

        // Assert
        Assert.False(redefinido);
    }
}
