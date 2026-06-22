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

    [Fact]
    public async Task PrestadorObterPerfilPublicoAsync_DeveRetornarCertificacoesEPortfolios()
    {
        var repository = new FakePrestadorRepository();
        var service = new PrestadorService(repository);
        var usuario = EntidadeFactory.CriarUsuario(nome: "Maria Silva", tipo: TipoUsuario.Prestador);
        var cidade = EntidadeFactory.CriarCidade(nome: "Blumenau", estado: "SC");
        var endereco = EntidadeFactory.CriarEndereco(usuarioId: usuario.Id, cidadeId: cidade.Id);
        EntidadeFactory.DefinirNavegacao(endereco, nameof(endereco.Cidade), cidade);
        usuario.DefinirEndereco(endereco);

        var prestador = EntidadeFactory.CriarPrestador(usuarioId: usuario.Id);
        EntidadeFactory.DefinirNavegacao(prestador, nameof(prestador.Usuario), usuario);

        var certificacao = new HomeTask.Domain.Entidades.Certificacao();
        certificacao.DefinirDados(prestador.Id, "Curso Profissional", "Instituto X", new DateTime(2025, 1, 1), null, "/arquivos/cert.pdf", DateTime.UtcNow);
        EntidadeFactory.DefinirNavegacao(certificacao, nameof(certificacao.Verificada), true);

        var portfolio = new HomeTask.Domain.Entidades.Portfolio();
        portfolio.DefinirDados(prestador.Id, "/arquivos/portfolio.jpg", "Antes e Depois", "Descricao do trabalho", DateTime.UtcNow);
        EntidadeFactory.DefinirNavegacao(portfolio, nameof(portfolio.Ordem), 1);

        EntidadeFactory.DefinirNavegacao(prestador, nameof(prestador.Certificacoes), new List<HomeTask.Domain.Entidades.Certificacao> { certificacao });
        EntidadeFactory.DefinirNavegacao(prestador, nameof(prestador.Portfolios), new List<HomeTask.Domain.Entidades.Portfolio> { portfolio });
        EntidadeFactory.DefinirNavegacao(prestador, nameof(prestador.ServicosOferecidos), new List<HomeTask.Domain.Entidades.ServicoPrestador>());
        repository.Seed(prestador);

        var perfil = await service.ObterPerfilPublicoAsync(prestador.Id);

        Assert.NotNull(perfil);
        Assert.Equal("Maria Silva", perfil!.Nome);
        Assert.Equal("Blumenau", perfil.Cidade);
        Assert.Single(perfil.Certificacoes);
        Assert.Equal("Curso Profissional", perfil.Certificacoes[0].Nome);
        Assert.Single(perfil.Portfolios);
        Assert.Equal("Antes e Depois", perfil.Portfolios[0].Titulo);
    }

    [Fact]
    public async Task PrestadorAtualizarMediaAvaliacoesAsync_ComMenosDeCincoAvaliacoesNaoDeveNotificar()
    {
        var repository = new FakePrestadorRepository();
        var emailService = new FakeEmailService();
        var prestador = CriarPrestadorComUsuario();
        AdicionarAvaliacoes(prestador, 3, 3, 3, 3);
        repository.Seed(prestador);
        var service = new PrestadorService(repository, emailService, () => new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc));

        await service.AtualizarMediaAvaliacoesAsync(prestador.Id);

        Assert.Equal(4, prestador.TotalAvaliacoes);
        Assert.Equal(3m, prestador.MediaAvaliacoes);
        Assert.Null(prestador.DataPrimeiraNotificacaoBaixaAvaliacao);
        Assert.Equal(0, emailService.AvisosBaixaAvaliacaoEnviados);
    }

    [Fact]
    public async Task PrestadorAtualizarMediaAvaliacoesAsync_AoAtingirCincoAvaliacoesComMediaBaixaDeveNotificar()
    {
        var agora = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);
        var repository = new FakePrestadorRepository();
        var emailService = new FakeEmailService();
        var prestador = CriarPrestadorComUsuario();
        AdicionarAvaliacoes(prestador, 3, 3, 3, 3, 3);
        repository.Seed(prestador);
        var service = new PrestadorService(repository, emailService, () => agora);

        await service.AtualizarMediaAvaliacoesAsync(prestador.Id);

        Assert.Equal(agora, prestador.DataPrimeiraNotificacaoBaixaAvaliacao);
        Assert.Equal(5, prestador.TotalAvaliacoesNaNotificacao);
        Assert.Equal(1, emailService.AvisosBaixaAvaliacaoEnviados);
        Assert.Equal(StatusPrestador.Ativo, prestador.Status);
    }

    [Fact]
    public async Task PrestadorAtualizarMediaAvaliacoesAsync_NaoDeveDuplicarAvisoAntesDeCompletarNovaJanela()
    {
        var agora = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);
        var repository = new FakePrestadorRepository();
        var emailService = new FakeEmailService();
        var prestador = CriarPrestadorComUsuario();
        AdicionarAvaliacoes(prestador, 3, 3, 3, 3, 3, 3, 3);
        prestador.RegistrarObservacaoBaixaAvaliacao(agora.AddDays(-2), 5);
        repository.Seed(prestador);
        var service = new PrestadorService(repository, emailService, () => agora);

        await service.AtualizarMediaAvaliacoesAsync(prestador.Id);

        Assert.Equal(5, prestador.TotalAvaliacoesNaNotificacao);
        Assert.Equal(0, emailService.AvisosBaixaAvaliacaoEnviados);
        Assert.Equal(0, emailService.AvisosSuspensaoEnviados);
        Assert.Equal(StatusPrestador.Ativo, prestador.Status);
    }

    [Fact]
    public async Task PrestadorAtualizarMediaAvaliacoesAsync_QuandoMediaRecuperaDeveLimparObservacao()
    {
        var agora = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);
        var repository = new FakePrestadorRepository();
        var emailService = new FakeEmailService();
        var prestador = CriarPrestadorComUsuario();
        AdicionarAvaliacoes(prestador, 5, 5, 5, 5, 4);
        prestador.RegistrarObservacaoBaixaAvaliacao(agora.AddDays(-2), 5);
        repository.Seed(prestador);
        var service = new PrestadorService(repository, emailService, () => agora);

        await service.AtualizarMediaAvaliacoesAsync(prestador.Id);

        Assert.Null(prestador.DataPrimeiraNotificacaoBaixaAvaliacao);
        Assert.Null(prestador.TotalAvaliacoesNaNotificacao);
        Assert.Equal(0, emailService.AvisosSuspensaoEnviados);
    }

    [Fact]
    public async Task PrestadorAtualizarMediaAvaliacoesAsync_DeveSuspenderAoCompletarCincoNovasAvaliacoesSemRecuperarMedia()
    {
        var agora = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);
        var repository = new FakePrestadorRepository();
        var emailService = new FakeEmailService();
        var prestador = CriarPrestadorComUsuario();
        AdicionarAvaliacoes(prestador, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3);
        prestador.RegistrarObservacaoBaixaAvaliacao(agora.AddDays(-5), 5);
        repository.Seed(prestador);
        var service = new PrestadorService(repository, emailService, () => agora);

        await service.AtualizarMediaAvaliacoesAsync(prestador.Id);

        Assert.Equal(StatusPrestador.Suspenso, prestador.Status);
        Assert.Equal(agora, prestador.DataInicioSuspensao);
        Assert.Equal(agora.AddDays(7), prestador.DataFimSuspensao);
        Assert.Equal(1, emailService.AvisosSuspensaoEnviados);
    }

    [Fact]
    public async Task PrestadorAtualizarMediaAvaliacoesAsync_NaoDeveSuspenderQuandoMediaJaRecuperouMesmoComJanelaCompleta()
    {
        var agora = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);
        var repository = new FakePrestadorRepository();
        var emailService = new FakeEmailService();
        var prestador = CriarPrestadorComUsuario();
        AdicionarAvaliacoes(prestador, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5);
        prestador.RegistrarObservacaoBaixaAvaliacao(agora.AddDays(-5), 5);
        repository.Seed(prestador);
        var service = new PrestadorService(repository, emailService, () => agora);

        await service.AtualizarMediaAvaliacoesAsync(prestador.Id);

        Assert.Equal(StatusPrestador.Ativo, prestador.Status);
        Assert.Null(prestador.DataInicioSuspensao);
        Assert.Null(prestador.TotalAvaliacoesNaNotificacao);
        Assert.Equal(0, emailService.AvisosSuspensaoEnviados);
    }

    [Fact]
    public async Task ProcessarSuspensoesExpiradasAsync_DeveReativarPrestadorComPrazoEncerrado()
    {
        var agora = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);
        var repository = new FakePrestadorRepository();
        var emailService = new FakeEmailService();
        var prestadorExpirado = CriarPrestadorComUsuario(status: StatusPrestador.Suspenso);
        prestadorExpirado.RegistrarObservacaoBaixaAvaliacao(agora.AddDays(-10), 5);
        prestadorExpirado.AplicarSuspensaoTemporaria(agora.AddDays(-8), agora.AddDays(-1));
        var prestadorAtivo = CriarPrestadorComUsuario();
        repository.Seed(prestadorExpirado, prestadorAtivo);
        var service = new PrestadorService(repository, emailService, () => agora);

        var reativados = await service.ProcessarSuspensoesExpiradasAsync();

        Assert.Equal(1, reativados);
        Assert.Equal(StatusPrestador.Ativo, prestadorExpirado.Status);
        Assert.Null(prestadorExpirado.DataInicioSuspensao);
        Assert.Null(prestadorExpirado.DataFimSuspensao);
        Assert.Null(prestadorExpirado.TotalAvaliacoesNaNotificacao);
        Assert.NotNull(prestadorExpirado.DataVerificacao);
    }

    [Fact]
    public async Task ProcessarSuspensoesExpiradasAsync_NaoDeveReativarPrestadorAindaNoPrazo()
    {
        var agora = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);
        var repository = new FakePrestadorRepository();
        var emailService = new FakeEmailService();
        var prestador = CriarPrestadorComUsuario(status: StatusPrestador.Suspenso);
        prestador.RegistrarObservacaoBaixaAvaliacao(agora.AddDays(-2), 5);
        prestador.AplicarSuspensaoTemporaria(agora.AddDays(-1), agora.AddDays(2));
        repository.Seed(prestador);
        var service = new PrestadorService(repository, emailService, () => agora);

        var reativados = await service.ProcessarSuspensoesExpiradasAsync();

        Assert.Equal(0, reativados);
        Assert.Equal(StatusPrestador.Suspenso, prestador.Status);
        Assert.NotNull(prestador.DataFimSuspensao);
    }

    private static HomeTask.Domain.Entidades.Prestador CriarPrestadorComUsuario(StatusPrestador status = StatusPrestador.Ativo)
    {
        var usuario = EntidadeFactory.CriarUsuario(nome: "Prestador Qualidade", tipo: TipoUsuario.Prestador, documento: $"{Random.Shared.NextInt64(10000000000, 99999999999)}");
        var prestador = EntidadeFactory.CriarPrestador(usuarioId: usuario.Id, status: status);
        EntidadeFactory.DefinirNavegacao(prestador, nameof(prestador.Usuario), usuario);
        EntidadeFactory.DefinirNavegacao(prestador, nameof(prestador.Avaliacoes), new List<HomeTask.Domain.Entidades.Avaliacao>());
        return prestador;
    }

    private static void AdicionarAvaliacoes(HomeTask.Domain.Entidades.Prestador prestador, params int[] notasPrestador)
    {
        var avaliacoes = (List<HomeTask.Domain.Entidades.Avaliacao>)prestador.Avaliacoes;
        foreach (var nota in notasPrestador)
        {
            avaliacoes.Add(EntidadeFactory.CriarAvaliacaoPrestador(
                Guid.NewGuid(),
                Guid.NewGuid(),
                prestador.Id,
                Guid.NewGuid(),
                nota));
        }
    }
}
