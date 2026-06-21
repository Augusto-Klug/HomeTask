using HomeTask.Application.Dtos;
using HomeTask.Application.Services;
using HomeTask.Domain.Enums;
using HomeTask.Tests.Helpers;

namespace HomeTask.Tests.Services;

public class CadastroServicesTests
{
    [Fact]
    public async Task ClienteCriarAsync_QuandoDtoValido_DevePersistirCliente()
    {
        // Arrange
        var repository = new FakeClienteRepository();
        var service = new ClienteService(repository);
        var dto = new ClienteDto { UsuarioId = Guid.NewGuid() };

        // Act
        var resultado = await service.CriarAsync(dto);

        // Assert
        Assert.Equal(dto.Id, resultado.Id);
        Assert.Equal(dto.UsuarioId, resultado.UsuarioId);
        Assert.Equal(1, repository.AdicionarChamadas);
        Assert.Equal(1, repository.SalvarChamadas);
    }

    [Fact]
    public async Task ClienteAtualizarAsync_QuandoDtoValido_DeveAtualizarCliente()
    {
        // Arrange
        var repository = new FakeClienteRepository();
        var service = new ClienteService(repository);
        var dto = new ClienteDto { Id = Guid.NewGuid(), UsuarioId = Guid.NewGuid() };

        // Act
        var resultado = await service.AtualizarAsync(dto);

        // Assert
        Assert.Equal(dto.Id, resultado.Id);
        Assert.Equal(1, repository.AtualizarChamadas);
        Assert.Equal(1, repository.SalvarChamadas);
    }

    [Fact]
    public async Task PrestadorCriarAsync_QuandoDtoValido_DeveSalvarComStatusEmAnalise()
    {
        // Arrange
        var repository = new FakePrestadorRepository();
        var service = new PrestadorService(repository);
        var dto = new PrestadorDto { UsuarioId = Guid.NewGuid(), Status = StatusPrestador.Ativo, Descricao = "Eletricista" };

        // Act
        var resultado = await service.CriarAsync(dto);

        // Assert
        Assert.Equal(dto.Id, resultado.Id);
        Assert.Equal(StatusPrestador.EmAnalise, resultado.Status);
        Assert.Equal(1, repository.AdicionarChamadas);
        Assert.Equal(1, repository.SalvarChamadas);
    }

    [Fact]
    public async Task PrestadorAtualizarStatusAsync_QuandoAtivar_DeveDefinirDataVerificacao()
    {
        // Arrange
        var repository = new FakePrestadorRepository();
        var prestador = EntidadeFactory.CriarPrestador(status: StatusPrestador.EmAnalise);
        repository.Seed(prestador);
        var service = new PrestadorService(repository);

        // Act
        await service.AtualizarStatusAsync(prestador.Id, StatusPrestador.Ativo);

        // Assert
        Assert.Equal(StatusPrestador.Ativo, prestador.Status);
        Assert.NotNull(prestador.DataVerificacao);
        Assert.Equal(1, repository.AtualizarChamadas);
        Assert.Equal(1, repository.SalvarChamadas);
    }

    [Fact]
    public async Task PrestadorObterRecebimentosAsync_DeveSomarServicosConcluidos()
    {
        var repository = new FakePrestadorRepository();
        var service = new PrestadorService(repository);
        var prestador = EntidadeFactory.CriarPrestador();
        var clienteUsuario = EntidadeFactory.CriarUsuario(nome: "Maria");
        var cliente = EntidadeFactory.CriarCliente();
        var cidade = EntidadeFactory.CriarCidade(nome: "Blumenau", estado: "SC");
        var endereco = EntidadeFactory.CriarEndereco(cidadeId: cidade.Id);
        var servico = EntidadeFactory.CriarServicoPrestador(prestadorId: prestador.Id, preco: 210);
        var agendamento = EntidadeFactory.CriarAgendamento(prestadorId: prestador.Id, clienteId: cliente.Id, status: StatusAgendamento.Concluido);

        EntidadeFactory.DefinirNavegacao(cliente, nameof(cliente.Usuario), clienteUsuario);
        EntidadeFactory.DefinirNavegacao(endereco, nameof(endereco.Cidade), cidade);
        EntidadeFactory.DefinirNavegacao(agendamento, nameof(agendamento.Cliente), cliente);
        EntidadeFactory.DefinirNavegacao(agendamento, nameof(agendamento.Endereco), endereco);
        EntidadeFactory.DefinirNavegacao(agendamento, nameof(agendamento.ValorTotal), 210m);
        EntidadeFactory.DefinirNavegacao(agendamento, nameof(agendamento.AgendamentoServicos), new List<HomeTask.Domain.Entidades.AgendamentoServico>
        {
            EntidadeFactory.CriarAgendamentoServico(agendamento.Id, servico.Id, 210)
        });
        ((List<HomeTask.Domain.Entidades.AgendamentoServico>)agendamento.AgendamentoServicos).ForEach(item =>
            EntidadeFactory.DefinirNavegacao(item, nameof(item.ServicoBase), servico));
        EntidadeFactory.DefinirNavegacao(agendamento, nameof(agendamento.DataConclusao), DateTime.UtcNow);

        repository.HistoricoServicos.Add(agendamento);

        var resultado = await service.ObterRecebimentosAsync(prestador.Id);

        Assert.Equal(210, resultado.SaldoRecebidoTotal);
        Assert.Equal(1, resultado.TotalServicosRecebidos);
        Assert.Single(resultado.ServicosRecebidos);
        Assert.Equal("Faxina completa", resultado.ServicosRecebidos[0].TituloServico);
        Assert.Equal("Maria", resultado.ServicosRecebidos[0].ClienteNome);
    }
}
