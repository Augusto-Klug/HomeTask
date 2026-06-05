using HomeTask.Domain.Enums;
using HomeTask.Infrastructure.Repositories;
using HomeTask.Tests.Helpers;

namespace HomeTask.Tests.Integration;

public class AgendamentoRepositoryIntegrationTests
{
    [Fact]
    [Trait("categoria", "integracao")]
    public async Task ObterPorIdAsync_QuandoAgendamentoExiste_DeveCarregarNavegacoesRelacionadas()
    {
        // Arrange
        using var sqlite = new SqliteTestContext();
        var repository = new AgendamentoRepository(sqlite.Db);
        var cidade = EntidadeFactory.CriarCidade();
        var usuarioCliente = EntidadeFactory.CriarUsuario(nome: "Cliente", tipo: TipoUsuario.Cliente, documento: "77777777777");
        var usuarioPrestador = EntidadeFactory.CriarUsuario(nome: "Prestador", tipo: TipoUsuario.Prestador, documento: "88888888888");
        var endereco = EntidadeFactory.CriarEndereco(usuarioId: usuarioCliente.Id, cidadeId: cidade.Id);
        var cliente = EntidadeFactory.CriarCliente(usuarioId: usuarioCliente.Id);
        var prestador = EntidadeFactory.CriarPrestador(usuarioId: usuarioPrestador.Id);
        var servico = EntidadeFactory.CriarServicoPrestador(prestadorId: prestador.Id, categoria: CategoriaServico.Faxina, preco: 150);
        var agendamento = EntidadeFactory.CriarAgendamento(clienteId: cliente.Id, prestadorId: prestador.Id, enderecoId: endereco.Id, status: StatusAgendamento.Solicitado);
        var agendamentoServico = EntidadeFactory.CriarAgendamentoServico(agendamento.Id, servico.Id, servico.PrecoBase);

        sqlite.Db.AddRange(cidade, usuarioCliente, usuarioPrestador, endereco, cliente, prestador, servico, agendamento, agendamentoServico);
        await sqlite.Db.SaveChangesAsync();

        // Act
        var resultado = await repository.ObterPorIdAsync(agendamento.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Cliente", resultado.Cliente.Usuario.Nome);
        Assert.Equal("Prestador", resultado.Prestador.Usuario.Nome);
        Assert.Equal("Blumenau", resultado.Endereco.Cidade.Nome);
        Assert.Single(resultado.AgendamentoServicos);
        Assert.Equal(servico.Id, resultado.AgendamentoServicos.First().ServicoBase.Id);
    }

    [Fact]
    [Trait("categoria", "integracao")]
    public async Task ObterEnderecoPrincipalDoClienteAsync_QuandoClienteTemEndereco_DeveRetornarEnderecoPorRelacionamento()
    {
        // Arrange
        using var sqlite = new SqliteTestContext();
        var repository = new AgendamentoRepository(sqlite.Db);
        var cidade = EntidadeFactory.CriarCidade();
        var usuario = EntidadeFactory.CriarUsuario(tipo: TipoUsuario.Cliente, documento: "99999999999");
        var cliente = EntidadeFactory.CriarCliente(usuarioId: usuario.Id);
        var endereco = EntidadeFactory.CriarEndereco(usuarioId: usuario.Id, cidadeId: cidade.Id, logradouro: "Rua Amazonas");

        sqlite.Db.AddRange(cidade, usuario, cliente, endereco);
        await sqlite.Db.SaveChangesAsync();

        // Act
        var resultado = await repository.ObterEnderecoPrincipalDoClienteAsync(cliente.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(endereco.Id, resultado.Id);
        Assert.Equal("Rua Amazonas", resultado.Logradouro);
    }
}
