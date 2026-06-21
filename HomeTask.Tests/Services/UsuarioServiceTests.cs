using HomeTask.Application.Dtos;
using HomeTask.Application.Services;
using HomeTask.Domain.Enums;
using HomeTask.Tests.Helpers;

namespace HomeTask.Tests.Services;

public class UsuarioServiceTests
{
    [Fact]
    public async Task CriarAsync_QuandoUsuarioAmbos_DeveCriarClientePrestadorEHashDaSenha()
    {
        // Arrange
        var repository = new FakeUsuarioRepository();
        var service = new UsuarioService(repository);
        var dto = new UsuarioDto
        {
            Nome = "Maria Silva",
            Email = "maria@teste.com",
            Documento = "12345678900",
            Telefone = "47999999999",
            TipoUsuario = TipoUsuario.Ambos,
            Ativo = true
        };

        // Act
        var resultado = await service.CriarAsync(dto, "senha-segura");
        var usuarioPersistido = Assert.IsType<HomeTask.Domain.Entidades.Usuario>(repository.UltimoAdicionado);

        // Assert
        Assert.Equal(dto.Id, resultado.Id);
        Assert.Equal(TipoUsuario.Ambos, resultado.TipoUsuario);
        Assert.NotEqual("senha-segura", usuarioPersistido.SenhaHash);
        Assert.NotNull(usuarioPersistido.Cliente);
        Assert.NotNull(usuarioPersistido.Prestador);
        Assert.Equal(StatusPrestador.EmAnalise, usuarioPersistido.Prestador.Status);
        Assert.Equal(1, repository.AdicionarChamadas);
        Assert.Equal(1, repository.SalvarChamadas);
    }

    [Fact]
    public async Task ValidarSenhaAsync_QuandoSenhaConfere_DeveRetornarTrue()
    {
        // Arrange
        var repository = new FakeUsuarioRepository();
        var service = new UsuarioService(repository);
        var dto = new UsuarioDto
        {
            Nome = "Joao Silva",
            Email = "joao@teste.com",
            Documento = "12345678900",
            TipoUsuario = TipoUsuario.Cliente,
            Ativo = true
        };
        await service.CriarAsync(dto, "senha-correta");

        // Act
        var senhaValida = await service.ValidarSenhaAsync(dto.Email, "senha-correta");
        var senhaInvalida = await service.ValidarSenhaAsync(dto.Email, "senha-errada");

        // Assert
        Assert.True(senhaValida);
        Assert.False(senhaInvalida);
    }

    [Fact]
    public async Task AtualizarPerfilAsync_QuandoUsuarioExiste_DeveAtualizarDadosBasicosEPrestador()
    {
        // Arrange
        var repository = new FakeUsuarioRepository();
        var usuario = EntidadeFactory.CriarUsuario(tipo: TipoUsuario.Prestador);
        var prestador = EntidadeFactory.CriarPrestador(usuarioId: usuario.Id, status: StatusPrestador.EmAnalise);
        var endereco = EntidadeFactory.CriarEndereco(usuarioId: usuario.Id, bairro: "Centro");
        usuario.DefinirPrestador(prestador);
        usuario.DefinirEndereco(endereco);
        repository.Seed(usuario);
        var service = new UsuarioService(repository);
        var dto = new PerfilDto
        {
            Nome = "Nome Atualizado",
            Documento = "98765432100",
            Telefone = "4711111111",
            Logradouro = "Rua Nova",
            Bairro = "Velha",
            Cep = "89000000",
            Descricao = "Perfil atualizado",
            RaioAtendimentoKm = 25,
            Status = StatusPrestador.Ativo
        };

        // Act
        var atualizado = await service.AtualizarPerfilAsync(usuario.Id, dto);

        // Assert
        Assert.True(atualizado);
        Assert.Equal("Nome Atualizado", usuario.Nome);
        Assert.Equal("98765432100", usuario.Documento);
        Assert.Equal("Rua Nova", usuario.Endereco!.Logradouro);
        Assert.Equal("Velha", usuario.Endereco.Bairro);
        Assert.Equal("Perfil atualizado", usuario.Prestador!.Descricao);
        Assert.Equal(StatusPrestador.Ativo, usuario.Prestador.Status);
        Assert.Equal(1, repository.AtualizarChamadas);
        Assert.Equal(1, repository.SalvarChamadas);
    }

    [Fact]
    public async Task AtualizarPerfilAsync_QuandoUsuarioNaoExiste_DeveRetornarFalseENaoSalvar()
    {
        // Arrange
        var repository = new FakeUsuarioRepository();
        var service = new UsuarioService(repository);

        // Act
        var atualizado = await service.AtualizarPerfilAsync(Guid.NewGuid(), new PerfilDto());

        // Assert
        Assert.False(atualizado);
        Assert.Equal(0, repository.AtualizarChamadas);
        Assert.Equal(0, repository.SalvarChamadas);
    }
}
