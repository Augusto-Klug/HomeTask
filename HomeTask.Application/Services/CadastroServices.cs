using HomeTask.Application.Dtos;
using HomeTask.Application.Interfaces;
using HomeTask.Application.Mappings;
using HomeTask.Domain.Enums;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.Repositories;

namespace HomeTask.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ClienteDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(id, cancellationToken);
        return cliente?.ParaDto();
    }

    public async Task<ClienteDto?> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);
        return cliente?.ParaDto();
    }

    public async Task<ClienteDto> CriarAsync(ClienteDto dto, CancellationToken cancellationToken = default)
    {
        var cliente = dto.ParaEntidade();
        await _clienteRepository.AdicionarAsync(cliente, cancellationToken);
        await _clienteRepository.SalvarAlteracoesAsync(cancellationToken);
        return cliente.ParaDto();
    }

    public async Task<ClienteDto> AtualizarAsync(ClienteDto dto, CancellationToken cancellationToken = default)
    {
        var cliente = dto.ParaEntidade();
        _clienteRepository.Atualizar(cliente);
        await _clienteRepository.SalvarAlteracoesAsync(cancellationToken);
        return cliente.ParaDto();
    }

    public async Task<IEnumerable<AgendamentoResumoDto>> ObterHistoricoAgendamentosAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var historico = await _clienteRepository.ObterHistoricoAgendamentosAsync(clienteId, cancellationToken);
        return historico.Select(a => a.ParaResumoDto());
    }

    public async Task AtualizarMediaAvaliacoesAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.ObterComAvaliacoesAsync(clienteId, cancellationToken);
        if (cliente == null)
            return;

        var avaliacoesVisiveis = cliente.AvaliacoesRecebidas.Where(a => a.Visivel).ToList();
        if (avaliacoesVisiveis.Count == 0)
        {
            cliente.AtualizarMetricasAvaliacao(0, 0);
        }
        else
        {
            cliente.AtualizarMetricasAvaliacao(
                (decimal)avaliacoesVisiveis.Average(a => a.Nota),
                avaliacoesVisiveis.Count);
        }

        _clienteRepository.Atualizar(cliente);
        await _clienteRepository.SalvarAlteracoesAsync(cancellationToken);
    }

    public async Task<ClientePerfilPublicoDto?> ObterPerfilPublicoAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(clienteId, cancellationToken);
        if (cliente == null)
            return null;

        var totalServicosContratados = await _clienteRepository.ObterTotalServicosContratadosConcluidosAsync(clienteId, cancellationToken);

        return new ClientePerfilPublicoDto
        {
            Id = cliente.Id,
            Nome = cliente.Usuario.Nome,
            Cidade = cliente.Usuario.Endereco?.Cidade?.Nome,
            Estado = cliente.Usuario.Endereco?.Cidade?.Estado,
            MediaAvaliacoes = cliente.MediaAvaliacoes,
            TotalAvaliacoes = cliente.TotalAvaliacoes,
            TotalServicosContratados = totalServicosContratados
        };
    }
}

public class PrestadorService : IPrestadorService
{
    private const int QuantidadeMinimaAvaliacoesParaRegra = 5;
    private static readonly TimeSpan DuracaoSuspensaoBaixaAvaliacao = TimeSpan.FromDays(7);
    private readonly IPrestadorRepository _prestadorRepository;
    private readonly IEmailService _emailService;
    private readonly Func<DateTime> _utcNowProvider;

    public PrestadorService(IPrestadorRepository prestadorRepository)
        : this(prestadorRepository, NullEmailService.Instance, static () => DateTime.UtcNow)
    {
    }

    public PrestadorService(IPrestadorRepository prestadorRepository, IEmailService emailService)
        : this(prestadorRepository, emailService, static () => DateTime.UtcNow)
    {
    }

    public PrestadorService(IPrestadorRepository prestadorRepository, IEmailService emailService, Func<DateTime> utcNowProvider)
    {
        _prestadorRepository = prestadorRepository;
        _emailService = emailService;
        _utcNowProvider = utcNowProvider;
    }

    public async Task<PrestadorDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var prestador = await _prestadorRepository.ObterPorIdAsync(id, cancellationToken);
        return prestador?.ParaDto();
    }

    public async Task<PrestadorDto?> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var prestador = await _prestadorRepository.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);
        return prestador?.ParaDto();
    }

    public async Task<PrestadorDto> CriarAsync(PrestadorDto dto, CancellationToken cancellationToken = default)
    {
        var prestador = dto.ParaEntidade();
        prestador.DefinirStatus(StatusPrestador.EmAnalise);
        await _prestadorRepository.AdicionarAsync(prestador, cancellationToken);
        await _prestadorRepository.SalvarAlteracoesAsync(cancellationToken);
        return prestador.ParaDto();
    }

    public async Task<PrestadorDto> AtualizarAsync(PrestadorDto dto, CancellationToken cancellationToken = default)
    {
        var prestador = dto.ParaEntidade();
        _prestadorRepository.Atualizar(prestador);
        await _prestadorRepository.SalvarAlteracoesAsync(cancellationToken);
        return prestador.ParaDto();
    }

    public async Task<IEnumerable<PrestadorDto>> BuscarAsync(CategoriaServico? categoria, string? cidade, DateTime? dataDisponivel, CancellationToken cancellationToken = default)
    {
        var prestadores = await _prestadorRepository.BuscarAsync(categoria, cidade, dataDisponivel, cancellationToken);
        return prestadores.Select(p => p.ParaDto());
    }

    public async Task<IEnumerable<AgendamentoResumoDto>> ObterHistoricoServicosAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        var historico = await _prestadorRepository.ObterHistoricoServicosAsync(prestadorId, cancellationToken);
        return historico.Select(a => a.ParaResumoDto());
    }

    public async Task<PrestadorRecebimentosResumoDto> ObterRecebimentosAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        var historico = await _prestadorRepository.ObterHistoricoServicosAsync(prestadorId, cancellationToken);
        var servicosRecebidos = historico
            .Where(a => a.Status == StatusAgendamento.Concluido)
            .Select(MapearRecebimento)
            .Where(item => item != null)
            .Cast<PrestadorRecebimentoItemDto>()
            .OrderByDescending(item => item.DataConclusao ?? DateTime.MinValue)
            .ToList();

        return new PrestadorRecebimentosResumoDto
        {
            SaldoRecebidoTotal = servicosRecebidos.Sum(item => item.ValorRecebido),
            TotalServicosRecebidos = servicosRecebidos.Count,
            ServicosRecebidos = servicosRecebidos
        };
    }

    public async Task AtualizarMediaAvaliacoesAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        var prestador = await _prestadorRepository.ObterComAvaliacoesAsync(prestadorId, cancellationToken);
        if (prestador == null)
            return;

        var agora = _utcNowProvider();
        var avaliacoesVisiveis = prestador.Avaliacoes.Where(a => a.Visivel).ToList();
        var totalAvaliacoes = avaliacoesVisiveis.Count;
        var mediaAvaliacoes = totalAvaliacoes == 0
            ? 0
            : (decimal)avaliacoesVisiveis.Average(a => a.NotaPrestador);

        prestador.AtualizarMetricasAvaliacao(mediaAvaliacoes, totalAvaliacoes);

        if (prestador.Status != StatusPrestador.Suspenso)
        {
            if (totalAvaliacoes >= QuantidadeMinimaAvaliacoesParaRegra && mediaAvaliacoes < 4)
            {
                if (!prestador.TotalAvaliacoesNaNotificacao.HasValue)
                {
                    prestador.RegistrarObservacaoBaixaAvaliacao(agora, totalAvaliacoes);

                    if (!string.IsNullOrWhiteSpace(prestador.Usuario?.Email))
                    {
                        await _emailService.EnviarAvisoBaixaAvaliacaoPrestadorAsync(
                            prestador.Usuario.Email,
                            prestador.Usuario.Nome,
                            mediaAvaliacoes,
                            totalAvaliacoes,
                            cancellationToken);
                    }
                }
                else
                {
                    var novasAvaliacoesDesdeAviso = totalAvaliacoes - prestador.TotalAvaliacoesNaNotificacao.Value;
                    if (novasAvaliacoesDesdeAviso >= QuantidadeMinimaAvaliacoesParaRegra)
                    {
                        var dataFimSuspensao = agora.Add(DuracaoSuspensaoBaixaAvaliacao);
                        prestador.AplicarSuspensaoTemporaria(agora, dataFimSuspensao);

                        if (!string.IsNullOrWhiteSpace(prestador.Usuario?.Email))
                        {
                            await _emailService.EnviarAvisoSuspensaoPrestadorAsync(
                                prestador.Usuario.Email,
                                prestador.Usuario.Nome,
                                dataFimSuspensao,
                                cancellationToken);
                        }
                    }
                }
            }
            else if (mediaAvaliacoes >= 4)
            {
                prestador.LimparObservacaoBaixaAvaliacao();
            }
        }

        _prestadorRepository.Atualizar(prestador);
        await _prestadorRepository.SalvarAlteracoesAsync(cancellationToken);
    }

    public async Task<PrestadorPerfilPublicoDto?> ObterPerfilPublicoAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        var prestador = await _prestadorRepository.ObterPorIdAsync(prestadorId, cancellationToken);
        if (prestador == null)
            return null;

        var historico = await _prestadorRepository.ObterHistoricoServicosAsync(prestadorId, cancellationToken);

        return new PrestadorPerfilPublicoDto
        {
            Id = prestador.Id,
            Nome = prestador.Usuario.Nome,
            Descricao = prestador.Descricao,
            Cidade = prestador.Usuario.Endereco?.Cidade?.Nome,
            Estado = prestador.Usuario.Endereco?.Cidade?.Estado,
            MediaAvaliacoes = prestador.MediaAvaliacoes,
            TotalAvaliacoes = prestador.TotalAvaliacoes,
            TotalServicosConcluidos = prestador.TotalServicosConcluidos,
            Certificacoes = prestador.Certificacoes
                .OrderByDescending(c => c.Verificada)
                .ThenBy(c => c.Nome)
                .Select(c => c.ParaDto())
                .ToList(),
            Portfolios = prestador.Portfolios
                .OrderBy(p => p.Ordem)
                .ThenByDescending(p => p.DataCadastro)
                .Select(p => p.ParaDto())
                .ToList(),
            ServicosOferecidos = prestador.ServicosOferecidos
                .Where(s => s.Ativo)
                .Select(s => s.ParaDto())
                .ToList(),
            HistoricoConcluido = historico
                .Where(a => a.Status == StatusAgendamento.Concluido)
                .Select(MapearHistoricoPublico)
                .Where(item => item != null)
                .Cast<PrestadorHistoricoPublicoDto>()
                .ToList()
        };
    }

    public async Task AtualizarStatusAsync(Guid prestadorId, StatusPrestador status, CancellationToken cancellationToken = default)
    {
        var prestador = await _prestadorRepository.ObterPorIdAsync(prestadorId, cancellationToken);
        if (prestador == null)
            return;

        prestador.DefinirStatus(status);
        if (status == StatusPrestador.Ativo)
            prestador.DefinirDataVerificacao(DateTime.UtcNow);

        _prestadorRepository.Atualizar(prestador);
        await _prestadorRepository.SalvarAlteracoesAsync(cancellationToken);
    }

    public async Task<int> ProcessarSuspensoesExpiradasAsync(CancellationToken cancellationToken = default)
    {
        var agora = _utcNowProvider();
        var prestadoresSuspensos = await _prestadorRepository.ObterSuspensosComSuspensaoExpiradaAsync(agora, cancellationToken);

        if (prestadoresSuspensos.Count == 0)
            return 0;

        foreach (var prestador in prestadoresSuspensos)
        {
            prestador.EncerrarSuspensaoTemporaria();
            prestador.DefinirDataVerificacao(agora);
            _prestadorRepository.Atualizar(prestador);
        }

        await _prestadorRepository.SalvarAlteracoesAsync(cancellationToken);
        return prestadoresSuspensos.Count;
    }

    private static PrestadorHistoricoPublicoDto? MapearHistoricoPublico(Agendamento agendamento)
    {
        var servicoPrestador = agendamento.AgendamentoServicos
            .Select(item => item.ServicoBase)
            .OfType<ServicoPrestador>()
            .FirstOrDefault();

        if (servicoPrestador == null)
            return null;

        return new PrestadorHistoricoPublicoDto
        {
            AgendamentoId = agendamento.Id,
            ServicoPrestadorId = servicoPrestador.Id,
            TituloServico = servicoPrestador.Titulo,
            DataHoraAgendada = agendamento.DataHoraAgendada,
            Cidade = agendamento.Endereco?.Cidade?.Nome ?? string.Empty,
            Estado = agendamento.Endereco?.Cidade?.Estado ?? string.Empty,
            NotaServico = agendamento.Avaliacao?.NotaServico,
            NotaPrestador = agendamento.Avaliacao?.NotaPrestador
        };
    }

    private static PrestadorRecebimentoItemDto? MapearRecebimento(Agendamento agendamento)
    {
        var servicoPrestador = agendamento.AgendamentoServicos
            .Select(item => item.ServicoBase)
            .OfType<ServicoPrestador>()
            .FirstOrDefault();

        if (servicoPrestador == null)
            return null;

        return new PrestadorRecebimentoItemDto
        {
            AgendamentoId = agendamento.Id,
            TituloServico = servicoPrestador.Titulo,
            ClienteNome = agendamento.Cliente?.Usuario?.Nome ?? string.Empty,
            DataConclusao = agendamento.DataConclusao,
            ValorRecebido = agendamento.ValorTotal,
            Cidade = agendamento.Endereco?.Cidade?.Nome ?? string.Empty,
            Estado = agendamento.Endereco?.Cidade?.Estado ?? string.Empty
        };
    }

    private sealed class NullEmailService : IEmailService
    {
        public static NullEmailService Instance { get; } = new();

        public Task EnviarResetSenhaAsync(string destinatario, string nome, string token, int validadeMinutos, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task EnviarAvisoBaixaAvaliacaoPrestadorAsync(string destinatario, string nome, decimal mediaAvaliacoes, int totalAvaliacoes, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task EnviarAvisoSuspensaoPrestadorAsync(string destinatario, string nome, DateTime dataFimSuspensao, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}

public class CidadeService : ICidadeService
{
    private readonly ICidadeRepository _cidadeRepository;

    public CidadeService(ICidadeRepository cidadeRepository)
    {
        _cidadeRepository = cidadeRepository;
    }

    public async Task<IEnumerable<CidadeDto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var cidades = await _cidadeRepository.ListarAsync(cancellationToken);
        return cidades.Select(c => c.ParaDto());
    }

    public async Task<IEnumerable<CidadeDto>> BuscarAsync(string termo, CancellationToken cancellationToken = default)
    {
        var cidades = await _cidadeRepository.BuscarAsync(termo, cancellationToken);
        return cidades.Select(c => c.ParaDto());
    }

    public async Task<CidadeDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cidade = await _cidadeRepository.ObterPorIdAsync(id, cancellationToken);
        return cidade?.ParaDto();
    }
}
