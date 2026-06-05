using HomeTask.Application.Dtos;
using HomeTask.Application.Services;
using HomeTask.Domain.Enums;
using HomeTask.Tests.Helpers;

namespace HomeTask.Tests.Services;

public class ServicoClienteServiceTests
{
    [Fact]
    public async Task CriarAsync_QuandoDtoValido_DevePersistirPedidoServico()
    {
        // Arrange
        var repository = new FakeServicoClienteRepository();
        var service = new ServicoClienteService(repository);
        var clienteId = Guid.NewGuid();
        var dataDesejada = DateTime.UtcNow.AddDays(3);
        var dto = new ServicoClienteDto
        {
            ClienteId = clienteId,
            Categoria = CategoriaServico.Jardinagem,
            Titulo = "Poda de jardim",
            Descricao = "Preciso podar arbustos",
            PrecoBase = 150,
            UnidadeCobranca = FormatoCobranca.Total,
            DataDesejada = dataDesejada
        };

        // Act
        var resultado = await service.CriarAsync(dto);

        // Assert
        Assert.Equal(clienteId, resultado.ClienteId);
        Assert.Equal(TipoAnuncio.Pedido, resultado.TipoAnuncio);
        Assert.Equal(dataDesejada, resultado.DataDesejada);
        Assert.Equal(1, repository.AdicionarChamadas);
        Assert.Equal(1, repository.SalvarChamadas);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoDtoValido_DeveAtualizarPedidoServico()
    {
        // Arrange
        var repository = new FakeServicoClienteRepository();
        var service = new ServicoClienteService(repository);
        var dto = new ServicoClienteDto
        {
            Id = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            Categoria = CategoriaServico.Lavanderia,
            Titulo = "Lavar roupas",
            Descricao = "Roupas delicadas",
            PrecoBase = 70,
            UnidadeCobranca = FormatoCobranca.ACombinar
        };

        // Act
        var resultado = await service.AtualizarAsync(dto);

        // Assert
        Assert.Equal(dto.Id, resultado.Id);
        Assert.Equal(FormatoCobranca.ACombinar, resultado.UnidadeCobranca);
        Assert.Equal(1, repository.AtualizarChamadas);
        Assert.Equal(1, repository.SalvarChamadas);
    }

    [Fact]
    public async Task RemoverAsync_QuandoServicoNaoExiste_DeveRetornarFalseENaoSalvar()
    {
        // Arrange
        var repository = new FakeServicoClienteRepository();
        var service = new ServicoClienteService(repository);

        // Act
        var removido = await service.RemoverAsync(Guid.NewGuid());

        // Assert
        Assert.False(removido);
        Assert.Equal(0, repository.AtualizarChamadas);
        Assert.Equal(0, repository.SalvarChamadas);
    }

    [Fact]
    public async Task BuscarPedidosAsync_QuandoFiltrosInformados_DeveRetornarPedidosFiltrados()
    {
        // Arrange
        var repository = new FakeServicoClienteRepository();
        repository.Seed(
            EntidadeFactory.CriarServicoCliente(categoria: CategoriaServico.Jardinagem, preco: 100),
            EntidadeFactory.CriarServicoCliente(categoria: CategoriaServico.Faxina, preco: 300));
        var service = new ServicoClienteService(repository);

        // Act
        var resultado = (await service.BuscarPedidosAsync(CategoriaServico.Jardinagem, null, 120)).ToList();

        // Assert
        Assert.Single(resultado);
        Assert.Equal(CategoriaServico.Jardinagem, resultado[0].Categoria);
        Assert.True(resultado[0].PrecoBase <= 120);
    }
}
