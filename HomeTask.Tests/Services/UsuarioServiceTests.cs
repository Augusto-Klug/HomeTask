using HomeTask.Application.Dtos;
using HomeTask.Application.Services;
using HomeTask.Tests.Helpers;

namespace HomeTask.Tests.Services;

public class UsuarioServiceTests
{
    [Fact]
    public async Task AtualizarPerfilAsync_DeveAtualizarEmailEDadosDoEndereco()
    {
        var repository = new FakeUsuarioRepository();
        var service = new UsuarioService(repository);

        var cidadeOriginal = EntidadeFactory.CriarCidade(nome: "Blumenau", estado: "SC");
        var cidadeNova = EntidadeFactory.CriarCidade(nome: "Curitiba", estado: "PR");
        var usuario = EntidadeFactory.CriarUsuario(nome: "Joao");
        var endereco = EntidadeFactory.CriarEndereco(usuarioId: usuario.Id, cidadeId: cidadeOriginal.Id);
        EntidadeFactory.DefinirNavegacao(endereco, nameof(endereco.Cidade), cidadeOriginal);
        usuario.DefinirEndereco(endereco);
        repository.Seed(usuario);

        var dto = new PerfilDto
        {
            Nome = "Joao Atualizado",
            Email = "novo@email.com",
            Documento = "987.654.321-00",
            Telefone = "47988887777",
            Cep = "80000-000",
            Logradouro = "Rua Nova",
            Bairro = "Centro Civico",
            Estado = "PR",
            Cidade = "Curitiba",
            CidadeId = cidadeNova.Id
        };

        var sucesso = await service.AtualizarPerfilAsync(usuario.Id, dto);

        Assert.True(sucesso);
        Assert.Equal("Joao Atualizado", usuario.Nome);
        Assert.Equal("novo@email.com", usuario.Email);
        Assert.Equal("987.654.321-00", usuario.Documento);
        Assert.Equal("47988887777", usuario.Telefone);
        Assert.NotNull(usuario.Endereco);
        Assert.Equal(cidadeNova.Id, usuario.Endereco!.CidadeId);
        Assert.Equal("Rua Nova", usuario.Endereco.Logradouro);
        Assert.Equal("Centro Civico", usuario.Endereco.Bairro);
        Assert.Equal("80000-000", usuario.Endereco.Cep);
    }

    [Fact]
    public async Task ObterPerfilAsync_DeveRetornarCidadeIdDoEndereco()
    {
        var repository = new FakeUsuarioRepository();
        var service = new UsuarioService(repository);

        var cidade = EntidadeFactory.CriarCidade(nome: "Florianopolis", estado: "SC");
        var usuario = EntidadeFactory.CriarUsuario(nome: "Maria");
        var endereco = EntidadeFactory.CriarEndereco(usuarioId: usuario.Id, cidadeId: cidade.Id);
        EntidadeFactory.DefinirNavegacao(endereco, nameof(endereco.Cidade), cidade);
        usuario.DefinirEndereco(endereco);
        repository.Seed(usuario);

        var perfil = await service.ObterPerfilAsync(usuario.Id);

        Assert.NotNull(perfil);
        Assert.Equal(cidade.Id, perfil!.CidadeId);
        Assert.Equal("Florianopolis", perfil.Cidade);
        Assert.Equal("SC", perfil.Estado);
    }
}
