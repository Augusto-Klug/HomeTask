using HomeTask.Domain.Enums;
using HomeTask.Infrastructure.Repositories;
using HomeTask.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace HomeTask.Tests.Integration;

public class ServicoRepositoryIntegrationTests
{
    [Fact]
    [Trait("categoria", "integracao")]
    public async Task BuscarTodosPaginadoAsync_QuandoFiltrosInformados_DeveBuscarOfertasEPedidosNoSqlite()
    {
        // Arrange
        using var sqlite = new SqliteTestContext();
        var repository = new ServicoPrestadorRepository(sqlite.Db);
        var cidadeBlumenau = EntidadeFactory.CriarCidade(nome: "Blumenau", estado: "SC");
        var cidadeItajai = EntidadeFactory.CriarCidade(nome: "Itajai", estado: "SC");
        var usuarioPrestador = EntidadeFactory.CriarUsuario(nome: "Prestador Blumenau", tipo: TipoUsuario.Prestador, documento: "11111111111");
        var usuarioCliente = EntidadeFactory.CriarUsuario(nome: "Cliente Blumenau", tipo: TipoUsuario.Cliente, documento: "22222222222");
        var usuarioForaFiltro = EntidadeFactory.CriarUsuario(nome: "Prestador Itajai", tipo: TipoUsuario.Prestador, documento: "33333333333");
        var enderecoPrestador = EntidadeFactory.CriarEndereco(usuarioId: usuarioPrestador.Id, cidadeId: cidadeBlumenau.Id);
        var enderecoCliente = EntidadeFactory.CriarEndereco(usuarioId: usuarioCliente.Id, cidadeId: cidadeBlumenau.Id);
        var enderecoForaFiltro = EntidadeFactory.CriarEndereco(usuarioId: usuarioForaFiltro.Id, cidadeId: cidadeItajai.Id);
        var prestador = EntidadeFactory.CriarPrestador(usuarioId: usuarioPrestador.Id, media: 4.5m);
        var cliente = EntidadeFactory.CriarCliente(usuarioId: usuarioCliente.Id);
        var prestadorForaFiltro = EntidadeFactory.CriarPrestador(usuarioId: usuarioForaFiltro.Id);
        var oferta = EntidadeFactory.CriarServicoPrestador(prestadorId: prestador.Id, categoria: CategoriaServico.Faxina, preco: 90, dataCriacao: DateTime.UtcNow.AddHours(-2));
        var pedido = EntidadeFactory.CriarServicoCliente(clienteId: cliente.Id, categoria: CategoriaServico.Faxina, preco: 70, dataCriacao: DateTime.UtcNow.AddHours(-1));
        var foraFiltro = EntidadeFactory.CriarServicoPrestador(prestadorId: prestadorForaFiltro.Id, categoria: CategoriaServico.Reparos, preco: 50);

        sqlite.Db.AddRange(cidadeBlumenau, cidadeItajai, usuarioPrestador, usuarioCliente, usuarioForaFiltro, enderecoPrestador, enderecoCliente, enderecoForaFiltro, prestador, cliente, prestadorForaFiltro, oferta, pedido, foraFiltro);
        await sqlite.Db.SaveChangesAsync();

        // Act
        var resultado = await repository.BuscarTodosPaginadoAsync(CategoriaServico.Faxina, "Blumenau", 100, null, null, 1, 10);

        // Assert
        Assert.Equal(2, resultado.TotalRegistros);
        Assert.Equal(1, resultado.TotalPaginas);
        Assert.Equal(10, resultado.TamanhoPagina);
        Assert.Collection(resultado.Itens,
            primeiro => Assert.Equal(pedido.Id, primeiro.Id),
            segundo => Assert.Equal(oferta.Id, segundo.Id));
    }

    [Fact]
    [Trait("categoria", "integracao")]
    public async Task BuscarAsync_QuandoPrestadorInativo_DeveIgnorarServicoMesmoComFiltrosValidos()
    {
        // Arrange
        using var sqlite = new SqliteTestContext();
        var repository = new ServicoPrestadorRepository(sqlite.Db);
        var cidade = EntidadeFactory.CriarCidade();
        var usuarioAtivo = EntidadeFactory.CriarUsuario(tipo: TipoUsuario.Prestador, documento: "44444444444");
        var usuarioAnalise = EntidadeFactory.CriarUsuario(tipo: TipoUsuario.Prestador, documento: "55555555555");
        var enderecoAtivo = EntidadeFactory.CriarEndereco(usuarioId: usuarioAtivo.Id, cidadeId: cidade.Id);
        var enderecoAnalise = EntidadeFactory.CriarEndereco(usuarioId: usuarioAnalise.Id, cidadeId: cidade.Id);
        var prestadorAtivo = EntidadeFactory.CriarPrestador(usuarioId: usuarioAtivo.Id, status: StatusPrestador.Ativo, media: 5);
        var prestadorAnalise = EntidadeFactory.CriarPrestador(usuarioId: usuarioAnalise.Id, status: StatusPrestador.EmAnalise, media: 5);
        var servicoAtivo = EntidadeFactory.CriarServicoPrestador(prestadorId: prestadorAtivo.Id, categoria: CategoriaServico.Faxina, preco: 100);
        var servicoAnalise = EntidadeFactory.CriarServicoPrestador(prestadorId: prestadorAnalise.Id, categoria: CategoriaServico.Faxina, preco: 80);

        sqlite.Db.AddRange(cidade, usuarioAtivo, usuarioAnalise, enderecoAtivo, enderecoAnalise, prestadorAtivo, prestadorAnalise, servicoAtivo, servicoAnalise);
        await sqlite.Db.SaveChangesAsync();

        // Act
        var resultado = (await repository.BuscarAsync(CategoriaServico.Faxina, "Blumenau", 150)).ToList();

        // Assert
        Assert.Single(resultado);
        Assert.Equal(servicoAtivo.Id, resultado[0].Id);
    }

    [Fact]
    [Trait("categoria", "integracao")]
    public async Task BuscarTodosPaginadoAsync_QuandoUsuarioAtualEhDonoDoServico_DeveExcluirPropriosAnuncios()
    {
        using var sqlite = new SqliteTestContext();
        var repository = new ServicoPrestadorRepository(sqlite.Db);
        var cidade = EntidadeFactory.CriarCidade();
        var usuarioAtual = EntidadeFactory.CriarUsuario(nome: "Prestador Atual", tipo: TipoUsuario.Prestador, documento: "77777777771");
        var usuarioOutro = EntidadeFactory.CriarUsuario(nome: "Outro Prestador", tipo: TipoUsuario.Prestador, documento: "77777777772");
        var usuarioCliente = EntidadeFactory.CriarUsuario(nome: "Cliente Atual", tipo: TipoUsuario.Cliente, documento: "77777777773");
        var enderecoAtual = EntidadeFactory.CriarEndereco(usuarioId: usuarioAtual.Id, cidadeId: cidade.Id);
        var enderecoOutro = EntidadeFactory.CriarEndereco(usuarioId: usuarioOutro.Id, cidadeId: cidade.Id);
        var enderecoCliente = EntidadeFactory.CriarEndereco(usuarioId: usuarioCliente.Id, cidadeId: cidade.Id);
        var prestadorAtual = EntidadeFactory.CriarPrestador(usuarioId: usuarioAtual.Id);
        var prestadorOutro = EntidadeFactory.CriarPrestador(usuarioId: usuarioOutro.Id);
        var clienteAtual = EntidadeFactory.CriarCliente(usuarioId: usuarioCliente.Id);
        var servicoProprio = EntidadeFactory.CriarServicoPrestador(prestadorId: prestadorAtual.Id, categoria: CategoriaServico.Jardinagem, preco: 120, dataCriacao: DateTime.UtcNow.AddMinutes(-5));
        var servicoOutro = EntidadeFactory.CriarServicoPrestador(prestadorId: prestadorOutro.Id, categoria: CategoriaServico.Jardinagem, preco: 130, dataCriacao: DateTime.UtcNow.AddMinutes(-4));
        var pedidoProprio = EntidadeFactory.CriarServicoCliente(clienteId: clienteAtual.Id, categoria: CategoriaServico.Jardinagem, preco: 80, dataCriacao: DateTime.UtcNow.AddMinutes(-3));

        sqlite.Db.AddRange(cidade, usuarioAtual, usuarioOutro, usuarioCliente, enderecoAtual, enderecoOutro, enderecoCliente, prestadorAtual, prestadorOutro, clienteAtual, servicoProprio, servicoOutro, pedidoProprio);
        await sqlite.Db.SaveChangesAsync();

        var resultadoPrestador = await repository.BuscarTodosPaginadoAsync(CategoriaServico.Jardinagem, "Blumenau", null, null, usuarioAtual.Id, 1, 10);
        var resultadoCliente = await repository.BuscarTodosPaginadoAsync(CategoriaServico.Jardinagem, "Blumenau", null, null, usuarioCliente.Id, 1, 10);

        Assert.DoesNotContain(resultadoPrestador.Itens, item => item.Id == servicoProprio.Id);
        Assert.Contains(resultadoPrestador.Itens, item => item.Id == servicoOutro.Id);
        Assert.Contains(resultadoPrestador.Itens, item => item.Id == pedidoProprio.Id);

        Assert.DoesNotContain(resultadoCliente.Itens, item => item.Id == pedidoProprio.Id);
        Assert.Contains(resultadoCliente.Itens, item => item.Id == servicoProprio.Id);
        Assert.Contains(resultadoCliente.Itens, item => item.Id == servicoOutro.Id);
    }

    [Fact]
    [Trait("categoria", "integracao")]
    public async Task BuscarTodosPaginadoAsync_QuandoTipoAnuncioInformado_DeveFiltrarOrigemDoAnuncio()
    {
        using var sqlite = new SqliteTestContext();
        var repository = new ServicoPrestadorRepository(sqlite.Db);
        var cidade = EntidadeFactory.CriarCidade();
        var usuarioPrestador = EntidadeFactory.CriarUsuario(tipo: TipoUsuario.Prestador, documento: "88888888881");
        var usuarioCliente = EntidadeFactory.CriarUsuario(tipo: TipoUsuario.Cliente, documento: "88888888882");
        var enderecoPrestador = EntidadeFactory.CriarEndereco(usuarioId: usuarioPrestador.Id, cidadeId: cidade.Id);
        var enderecoCliente = EntidadeFactory.CriarEndereco(usuarioId: usuarioCliente.Id, cidadeId: cidade.Id);
        var prestador = EntidadeFactory.CriarPrestador(usuarioId: usuarioPrestador.Id);
        var cliente = EntidadeFactory.CriarCliente(usuarioId: usuarioCliente.Id);
        var oferta = EntidadeFactory.CriarServicoPrestador(prestadorId: prestador.Id, categoria: CategoriaServico.Faxina, preco: 120);
        var pedido = EntidadeFactory.CriarServicoCliente(clienteId: cliente.Id, categoria: CategoriaServico.Faxina, preco: 80);

        sqlite.Db.AddRange(cidade, usuarioPrestador, usuarioCliente, enderecoPrestador, enderecoCliente, prestador, cliente, oferta, pedido);
        await sqlite.Db.SaveChangesAsync();

        var ofertas = await repository.BuscarTodosPaginadoAsync(CategoriaServico.Faxina, "Blumenau", null, TipoAnuncio.Oferta, null, 1, 10);
        var pedidos = await repository.BuscarTodosPaginadoAsync(CategoriaServico.Faxina, "Blumenau", null, TipoAnuncio.Pedido, null, 1, 10);

        Assert.Single(ofertas.Itens);
        Assert.Equal(oferta.Id, ofertas.Itens.Single().Id);
        Assert.Single(pedidos.Itens);
        Assert.Equal(pedido.Id, pedidos.Itens.Single().Id);
    }

    [Fact]
    [Trait("categoria", "integracao")]
    public async Task ServicoClienteRepository_BuscarPedidosAsync_DeveFiltrarPorCategoriaCidadeEPreco()
    {
        // Arrange
        using var sqlite = new SqliteTestContext();
        var repository = new ServicoClienteRepository(sqlite.Db);
        var cidade = EntidadeFactory.CriarCidade();
        var usuario = EntidadeFactory.CriarUsuario(tipo: TipoUsuario.Cliente, documento: "66666666666");
        var endereco = EntidadeFactory.CriarEndereco(usuarioId: usuario.Id, cidadeId: cidade.Id);
        var cliente = EntidadeFactory.CriarCliente(usuarioId: usuario.Id);
        var pedidoValido = EntidadeFactory.CriarServicoCliente(clienteId: cliente.Id, categoria: CategoriaServico.Jardinagem, preco: 120);
        var pedidoCaro = EntidadeFactory.CriarServicoCliente(clienteId: cliente.Id, categoria: CategoriaServico.Jardinagem, preco: 300);

        sqlite.Db.AddRange(cidade, usuario, endereco, cliente, pedidoValido, pedidoCaro);
        await sqlite.Db.SaveChangesAsync();

        // Act
        var resultado = (await repository.BuscarPedidosAsync(CategoriaServico.Jardinagem, "Blumenau", 150)).ToList();

        // Assert
        Assert.Single(resultado);
        Assert.Equal(pedidoValido.Id, resultado[0].Id);
    }

    [Fact]
    [Trait("categoria", "integracao")]
    public async Task PrestadorRepository_ObterSuspensosComSuspensaoExpiradaAsync_DeveRetornarApenasSuspensosExpirados()
    {
        using var sqlite = new SqliteTestContext();
        var repository = new PrestadorRepository(sqlite.Db);
        var cidade = EntidadeFactory.CriarCidade();
        var usuarioExpirado = EntidadeFactory.CriarUsuario(tipo: TipoUsuario.Prestador, documento: "99999999981");
        var usuarioAtivo = EntidadeFactory.CriarUsuario(tipo: TipoUsuario.Prestador, documento: "99999999982");
        var usuarioFuturo = EntidadeFactory.CriarUsuario(tipo: TipoUsuario.Prestador, documento: "99999999983");
        var enderecoExpirado = EntidadeFactory.CriarEndereco(usuarioId: usuarioExpirado.Id, cidadeId: cidade.Id);
        var enderecoAtivo = EntidadeFactory.CriarEndereco(usuarioId: usuarioAtivo.Id, cidadeId: cidade.Id);
        var enderecoFuturo = EntidadeFactory.CriarEndereco(usuarioId: usuarioFuturo.Id, cidadeId: cidade.Id);
        var prestadorExpirado = EntidadeFactory.CriarPrestador(usuarioId: usuarioExpirado.Id, status: StatusPrestador.Suspenso);
        var prestadorAtivo = EntidadeFactory.CriarPrestador(usuarioId: usuarioAtivo.Id, status: StatusPrestador.Ativo);
        var prestadorFuturo = EntidadeFactory.CriarPrestador(usuarioId: usuarioFuturo.Id, status: StatusPrestador.Suspenso);
        var agora = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);

        prestadorExpirado.AplicarSuspensaoTemporaria(agora.AddDays(-8), agora.AddDays(-1));
        prestadorFuturo.AplicarSuspensaoTemporaria(agora.AddDays(-1), agora.AddDays(2));

        sqlite.Db.AddRange(cidade, usuarioExpirado, usuarioAtivo, usuarioFuturo, enderecoExpirado, enderecoAtivo, enderecoFuturo, prestadorExpirado, prestadorAtivo, prestadorFuturo);
        await sqlite.Db.SaveChangesAsync();

        var resultado = await repository.ObterSuspensosComSuspensaoExpiradaAsync(agora);

        Assert.Single(resultado);
        Assert.Equal(prestadorExpirado.Id, resultado[0].Id);
    }

    [Fact]
    [Trait("categoria", "integracao")]
    public async Task PrestadorMap_DevePersistirCamposDaRegraDeSuspensaoAutomatica()
    {
        using var sqlite = new SqliteTestContext();
        var cidade = EntidadeFactory.CriarCidade();
        var usuario = EntidadeFactory.CriarUsuario(tipo: TipoUsuario.Prestador, documento: "99999999984");
        var endereco = EntidadeFactory.CriarEndereco(usuarioId: usuario.Id, cidadeId: cidade.Id);
        var prestador = EntidadeFactory.CriarPrestador(usuarioId: usuario.Id, status: StatusPrestador.Suspenso);
        var agora = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);

        prestador.RegistrarObservacaoBaixaAvaliacao(agora.AddDays(-5), 5);
        prestador.AplicarSuspensaoTemporaria(agora.AddDays(-2), agora.AddDays(5));

        sqlite.Db.AddRange(cidade, usuario, endereco, prestador);
        await sqlite.Db.SaveChangesAsync();
        sqlite.Db.ChangeTracker.Clear();

        var recarregado = await sqlite.Db.Prestadores.SingleAsync(p => p.Id == prestador.Id);

        Assert.Equal(agora.AddDays(-5), recarregado.DataPrimeiraNotificacaoBaixaAvaliacao);
        Assert.Equal(5, recarregado.TotalAvaliacoesNaNotificacao);
        Assert.Equal(agora.AddDays(-2), recarregado.DataInicioSuspensao);
        Assert.Equal(agora.AddDays(5), recarregado.DataFimSuspensao);
    }
}
