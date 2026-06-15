using HomeTask.Application.Dtos;
using HomeTask.Application.Services;
using HomeTask.Domain.Enums;
using HomeTask.Tests.Helpers;

namespace HomeTask.Tests.Services;

public class ServicoPrestadorServiceTests
{
    [Fact]
    public async Task CriarAsync_QuandoDtoValido_DevePersistirServicoERetornarDados()
    {
        // Arrange
        var repository = new FakeServicoPrestadorRepository();
        var service = new ServicoPrestadorService(repository);
        var prestadorId = Guid.NewGuid();
        var dto = new ServicoPrestadorDto
        {
            PrestadorId = prestadorId,
            Categoria = CategoriaServico.Faxina,
            Titulo = "Faxina completa",
            Descricao = "Limpeza residencial",
            PrecoBase = 180,
            UnidadeCobranca = FormatoCobranca.Total,
            DuracaoEstimadaMinutos = 180,
            AceitaPagamentoAposFinalizacao = true
        };

        // Act
        var resultado = await service.CriarAsync(dto);

        // Assert
        Assert.Equal(dto.Id, resultado.Id);
        Assert.Equal(prestadorId, resultado.PrestadorId);
        Assert.Equal("Faxina completa", resultado.Titulo);
        Assert.Equal(180, resultado.PrecoBase);
        Assert.Equal(1, repository.AdicionarChamadas);
        Assert.Equal(1, repository.SalvarChamadas);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoDtoValido_DeveAtualizarServicoERetornarDadosAlterados()
    {
        // Arrange
        var repository = new FakeServicoPrestadorRepository();
        var service = new ServicoPrestadorService(repository);
        var dto = new ServicoPrestadorDto
        {
            Id = Guid.NewGuid(),
            PrestadorId = Guid.NewGuid(),
            Categoria = CategoriaServico.Reparos,
            Titulo = "Reparo eletrico",
            Descricao = "Troca de tomadas",
            PrecoBase = 120,
            UnidadeCobranca = FormatoCobranca.PorHora,
            DuracaoEstimadaMinutos = 60
        };

        // Act
        var resultado = await service.AtualizarAsync(dto);

        // Assert
        Assert.Equal(dto.Id, resultado.Id);
        Assert.Equal(CategoriaServico.Reparos, resultado.Categoria);
        Assert.Equal(FormatoCobranca.PorHora, resultado.UnidadeCobranca);
        Assert.Equal(1, repository.AtualizarChamadas);
        Assert.Equal(1, repository.SalvarChamadas);
    }

    [Fact]
    public async Task RemoverAsync_QuandoServicoExiste_DeveDesativarServico()
    {
        // Arrange
        var repository = new FakeServicoPrestadorRepository();
        var service = new ServicoPrestadorService(repository);
        var servico = EntidadeFactory.CriarServicoPrestador();
        repository.Seed(servico);

        // Act
        var removido = await service.RemoverAsync(servico.Id);
        var atualizado = Assert.IsType<HomeTask.Domain.Entidades.ServicoPrestador>(repository.UltimoAtualizado);

        // Assert
        Assert.True(removido);
        Assert.False(atualizado.Ativo);
        Assert.Equal(1, repository.AtualizarChamadas);
        Assert.Equal(1, repository.SalvarChamadas);
    }

    [Fact]
    public async Task BuscarAsync_QuandoFiltrosInformados_DeveRetornarServicosDoRepositorio()
    {
        // Arrange
        var repository = new FakeServicoPrestadorRepository();
        repository.Seed(
            EntidadeFactory.CriarServicoPrestador(categoria: CategoriaServico.Faxina, preco: 90),
            EntidadeFactory.CriarServicoPrestador(categoria: CategoriaServico.Reparos, preco: 200));
        var service = new ServicoPrestadorService(repository);

        // Act
        var resultado = (await service.BuscarAsync(CategoriaServico.Faxina, null, 100)).ToList();

        // Assert
        Assert.Single(resultado);
        Assert.Equal(CategoriaServico.Faxina, resultado[0].Categoria);
        Assert.True(resultado[0].PrecoBase <= 100);
    }

    [Fact]
    public async Task AtualizarMediaAvaliacoesAsync_QuandoExistemAvaliacoes_DevePersistirMediaDoServico()
    {
        var repository = new FakeServicoPrestadorRepository();
        var service = new ServicoPrestadorService(repository);
        var servico = EntidadeFactory.CriarServicoPrestador();
        var avaliacaoA = new HomeTask.Domain.Entidades.Avaliacao();
        avaliacaoA.DefinirDados(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), servico.PrestadorId, servico.Id, 5, 4, null, DateTime.UtcNow, true);
        var avaliacaoB = new HomeTask.Domain.Entidades.Avaliacao();
        avaliacaoB.DefinirDados(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), servico.PrestadorId, servico.Id, 3, 5, null, DateTime.UtcNow, true);
        EntidadeFactory.DefinirNavegacao(servico, nameof(servico.Avaliacoes), new List<HomeTask.Domain.Entidades.Avaliacao> { avaliacaoA, avaliacaoB });
        repository.Seed(servico);

        await service.AtualizarMediaAvaliacoesAsync(servico.Id);

        var atualizado = Assert.IsType<HomeTask.Domain.Entidades.ServicoPrestador>(repository.UltimoAtualizado);
        Assert.Equal(4, atualizado.MediaAvaliacoes);
        Assert.Equal(2, atualizado.TotalAvaliacoes);
    }
}
