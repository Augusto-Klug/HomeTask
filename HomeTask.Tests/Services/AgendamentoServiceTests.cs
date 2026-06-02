using HomeTask.Application.Dtos;
using HomeTask.Application.Services;
using HomeTask.Domain.Enums;
using HomeTask.Tests.Helpers;

namespace HomeTask.Tests.Services;

public class AgendamentoServiceTests
{
    [Fact]
    public async Task CriarAsync_QuandoServicoPrestadorValido_DeveCriarSolicitacaoComValorDuracaoEEndereco()
    {
        // Arrange
        var agendamentoRepository = new FakeAgendamentoRepository();
        var prestadorRepository = new FakePrestadorRepository();
        var service = new AgendamentoService(agendamentoRepository, prestadorRepository);
        var clienteId = Guid.NewGuid();
        var prestadorId = Guid.NewGuid();
        var endereco = EntidadeFactory.CriarEndereco(usuarioId: Guid.NewGuid());
        var servico = EntidadeFactory.CriarServicoPrestador(prestadorId: prestadorId, preco: 250);
        agendamentoRepository.EnderecoPrincipal = endereco;
        agendamentoRepository.Servicos.Add(servico);
        var dto = new AgendamentoDto
        {
            ClienteId = clienteId,
            DataHoraAgendada = DateTime.UtcNow.AddDays(1),
            Observacoes = "Periodo da manha",
            ServicosOferecidosIds = [servico.Id]
        };

        // Act
        var resultado = await service.CriarAsync(dto);

        // Assert
        Assert.Equal(StatusAgendamento.Solicitado, resultado.Status);
        Assert.Equal(clienteId, resultado.ClienteId);
        Assert.Equal(prestadorId, resultado.PrestadorId);
        Assert.Equal(endereco.Id, resultado.EnderecoId);
        Assert.Equal(250, resultado.ValorTotal);
        Assert.Equal(120, resultado.DuracaoMinutos);
        Assert.Single(resultado.ServicosOferecidosIds);
        Assert.Equal(1, agendamentoRepository.AdicionarChamadas);
        Assert.Equal(1, agendamentoRepository.SalvarChamadas);
    }

    [Fact]
    public async Task CriarAsync_QuandoNaoEncontrarServico_DeveLancarErroENaoSalvar()
    {
        // Arrange
        var agendamentoRepository = new FakeAgendamentoRepository();
        var service = new AgendamentoService(agendamentoRepository, new FakePrestadorRepository());
        var dto = new AgendamentoDto { ClienteId = Guid.NewGuid(), ServicosOferecidosIds = [Guid.NewGuid()] };

        // Act
        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CriarAsync(dto));

        // Assert
        Assert.Equal("Nenhum serviço válido encontrado para o agendamento.", excecao.Message);
        Assert.Equal(0, agendamentoRepository.AdicionarChamadas);
        Assert.Equal(0, agendamentoRepository.SalvarChamadas);
    }

    [Fact]
    public async Task AceitarAsync_QuandoSolicitado_DeveAlterarStatusParaAceito()
    {
        // Arrange
        var agendamentoRepository = new FakeAgendamentoRepository();
        var agendamento = EntidadeFactory.CriarAgendamento(status: StatusAgendamento.Solicitado);
        agendamentoRepository.Seed(agendamento);
        var service = new AgendamentoService(agendamentoRepository, new FakePrestadorRepository());

        // Act
        var resultado = await service.AceitarAsync(agendamento.Id);

        // Assert
        Assert.Equal(StatusAgendamento.Aceito, resultado.Status);
        Assert.NotNull(resultado.DataResposta);
        Assert.Equal(1, agendamentoRepository.AtualizarChamadas);
        Assert.Equal(1, agendamentoRepository.SalvarChamadas);
    }

    [Fact]
    public async Task IniciarAsync_QuandoNaoAceito_DeveLancarErro()
    {
        // Arrange
        var agendamentoRepository = new FakeAgendamentoRepository();
        var agendamento = EntidadeFactory.CriarAgendamento(status: StatusAgendamento.Solicitado);
        agendamentoRepository.Seed(agendamento);
        var service = new AgendamentoService(agendamentoRepository, new FakePrestadorRepository());

        // Act
        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => service.IniciarAsync(agendamento.Id));

        // Assert
        Assert.Equal("Agendamento não pode ser iniciado neste status", excecao.Message);
        Assert.Equal(0, agendamentoRepository.AtualizarChamadas);
        Assert.Equal(0, agendamentoRepository.SalvarChamadas);
    }

    [Fact]
    public async Task ConcluirAsync_QuandoEmAndamento_DeveConcluirEIncrementarServicosDoPrestador()
    {
        // Arrange
        var agendamentoRepository = new FakeAgendamentoRepository();
        var prestadorRepository = new FakePrestadorRepository();
        var prestador = EntidadeFactory.CriarPrestador();
        var agendamento = EntidadeFactory.CriarAgendamento(prestadorId: prestador.Id, status: StatusAgendamento.EmAndamento);
        agendamentoRepository.Seed(agendamento);
        prestadorRepository.Seed(prestador);
        var service = new AgendamentoService(agendamentoRepository, prestadorRepository);

        // Act
        var resultado = await service.ConcluirAsync(agendamento.Id);

        // Assert
        Assert.Equal(StatusAgendamento.Concluido, resultado.Status);
        Assert.NotNull(resultado.DataConclusao);
        Assert.Equal(1, prestador.TotalServicosConcluidos);
        Assert.Equal(1, prestadorRepository.AtualizarChamadas);
        Assert.Equal(1, agendamentoRepository.AtualizarChamadas);
    }
}
