using HomeTask.Application.Dtos;
using HomeTask.Application.Interfaces;
using HomeTask.Application.Mappings;
using HomeTask.Domain.Enums;
using HomeTask.Domain.Repositories;

namespace HomeTask.Application.Services;

public class AgendamentoService : IAgendamentoService
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IPrestadorRepository _prestadorRepository;

    public AgendamentoService(IAgendamentoRepository agendamentoRepository, IPrestadorRepository prestadorRepository)
    {
        _agendamentoRepository = agendamentoRepository;
        _prestadorRepository = prestadorRepository;
    }

    public async Task<AgendamentoResumoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var agendamento = await _agendamentoRepository.ObterPorIdAsync(id, cancellationToken);
        return agendamento?.ParaResumoDto();
    }

    public async Task<AgendamentoDto> CriarAsync(AgendamentoDto dto, CancellationToken cancellationToken = default)
    {
        var agendamento = dto.ParaEntidade();
        var servicos = await _agendamentoRepository.ObterServicosPorIdsAsync(dto.ServicosOferecidosIds, cancellationToken);

        if (servicos.Count == 0)
            throw new InvalidOperationException("Nenhum serviço válido encontrado para o agendamento.");

        decimal valorTotal = 0;
        var duracaoTotal = 0;
        var prestadorId = agendamento.PrestadorId;
        var clienteId = agendamento.ClienteId;

        foreach (var servico in servicos)
        {
            if (servico is Domain.Entidades.ServicoPrestador sp)
            {
                if (prestadorId == Guid.Empty)
                    prestadorId = sp.PrestadorId;

                duracaoTotal += sp.DuracaoEstimadaMinutos ?? 0;
            }

            if (servico is Domain.Entidades.ServicoCliente sc && clienteId == Guid.Empty)
                clienteId = sc.ClienteId;

            var agendamentoServico = new Domain.Entidades.AgendamentoServico();
            agendamentoServico.DefinirDados(agendamento.Id, servico.Id, 1, servico.PrecoBase);
            agendamento.AdicionarServico(agendamentoServico);
            valorTotal += servico.PrecoBase;
        }

        var enderecoId = agendamento.EnderecoId;
        if (enderecoId == Guid.Empty)
        {
            var endereco = await _agendamentoRepository.ObterEnderecoPrincipalDoClienteAsync(clienteId, cancellationToken);
            if (endereco != null)
                enderecoId = endereco.Id;
        }

        if (enderecoId == Guid.Empty)
            throw new InvalidOperationException("Cliente não possui endereço cadastrado para o agendamento.");

        agendamento.DefinirDados(
            agendamento.Id,
            clienteId,
            prestadorId,
            agendamento.DataHoraAgendada,
            duracaoTotal > 0 ? duracaoTotal : agendamento.DuracaoMinutos,
            StatusAgendamento.Solicitado,
            enderecoId,
            agendamento.Observacoes,
            valorTotal,
            DateTime.UtcNow,
            null,
            null,
            null);

        await _agendamentoRepository.AdicionarAsync(agendamento, cancellationToken);
        await _agendamentoRepository.SalvarAlteracoesAsync(cancellationToken);
        return agendamento.ParaDto();
    }

    public async Task<AgendamentoDto> AceitarAsync(Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        var agendamento = await ObterAgendamentoOuFalhar(agendamentoId, cancellationToken);
        if (agendamento.Status != StatusAgendamento.Solicitado)
            throw new InvalidOperationException("Agendamento não pode ser aceito neste status");

        agendamento.Aceitar(DateTime.UtcNow);
        _agendamentoRepository.Atualizar(agendamento);
        await _agendamentoRepository.SalvarAlteracoesAsync(cancellationToken);
        return agendamento.ParaDto();
    }

    public async Task<AgendamentoDto> RecusarAsync(Guid agendamentoId, string motivo, CancellationToken cancellationToken = default)
    {
        var agendamento = await ObterAgendamentoOuFalhar(agendamentoId, cancellationToken);
        if (agendamento.Status != StatusAgendamento.Solicitado)
            throw new InvalidOperationException("Agendamento não pode ser recusado neste status");

        agendamento.Recusar(motivo, DateTime.UtcNow);
        _agendamentoRepository.Atualizar(agendamento);
        await _agendamentoRepository.SalvarAlteracoesAsync(cancellationToken);
        return agendamento.ParaDto();
    }

    public async Task<AgendamentoDto> IniciarAsync(Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        var agendamento = await ObterAgendamentoOuFalhar(agendamentoId, cancellationToken);
        if (agendamento.Status != StatusAgendamento.Aceito)
            throw new InvalidOperationException("Agendamento não pode ser iniciado neste status");

        agendamento.Iniciar();
        _agendamentoRepository.Atualizar(agendamento);
        await _agendamentoRepository.SalvarAlteracoesAsync(cancellationToken);
        return agendamento.ParaDto();
    }

    public async Task<AgendamentoDto> ConcluirAsync(Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        var agendamento = await ObterAgendamentoOuFalhar(agendamentoId, cancellationToken);
        if (agendamento.Status != StatusAgendamento.EmAndamento)
            throw new InvalidOperationException("Agendamento não pode ser concluído neste status");

        agendamento.Concluir(DateTime.UtcNow);

        var prestador = await _prestadorRepository.ObterPorIdAsync(agendamento.PrestadorId, cancellationToken);
        prestador?.IncrementarTotalServicosConcluidos();
        if (prestador != null)
            _prestadorRepository.Atualizar(prestador);

        _agendamentoRepository.Atualizar(agendamento);
        await _agendamentoRepository.SalvarAlteracoesAsync(cancellationToken);
        return agendamento.ParaDto();
    }

    public async Task<AgendamentoDto> CancelarAsync(Guid agendamentoId, string motivo, CancellationToken cancellationToken = default)
    {
        var agendamento = await ObterAgendamentoOuFalhar(agendamentoId, cancellationToken);
        if (agendamento.Status is StatusAgendamento.Concluido or StatusAgendamento.Cancelado)
            throw new InvalidOperationException("Agendamento não pode ser cancelado neste status");

        agendamento.Cancelar(motivo);
        _agendamentoRepository.Atualizar(agendamento);
        await _agendamentoRepository.SalvarAlteracoesAsync(cancellationToken);
        return agendamento.ParaDto();
    }

    public async Task<IEnumerable<AgendamentoResumoDto>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var agendamentos = await _agendamentoRepository.ObterPorClienteAsync(clienteId, cancellationToken);
        return agendamentos.Select(a => a.ParaResumoDto());
    }

    public async Task<IEnumerable<AgendamentoResumoDto>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        var agendamentos = await _agendamentoRepository.ObterPorPrestadorAsync(prestadorId, cancellationToken);
        return agendamentos.Select(a => a.ParaResumoDto());
    }

    public async Task<IEnumerable<AgendamentoResumoDto>> ObterSolicitacoesPendentesPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        var agendamentos = await _agendamentoRepository.ObterSolicitacoesPendentesPorPrestadorAsync(prestadorId, cancellationToken);
        return agendamentos.Select(a => a.ParaResumoDto());
    }

    public async Task<IEnumerable<AgendamentoResumoDto>> ObterPorStatusAsync(StatusAgendamento status, CancellationToken cancellationToken = default)
    {
        var agendamentos = await _agendamentoRepository.ObterPorStatusAsync(status, cancellationToken);
        return agendamentos.Select(a => a.ParaResumoDto());
    }

    private async Task<Domain.Entidades.Agendamento> ObterAgendamentoOuFalhar(Guid id, CancellationToken cancellationToken)
    {
        var agendamento = await _agendamentoRepository.ObterPorIdAsync(id, cancellationToken);
        return agendamento ?? throw new InvalidOperationException("Agendamento não encontrado");
    }
}

public class AvaliacaoService : IAvaliacaoService
{
    private readonly IAvaliacaoRepository _avaliacaoRepository;
    private readonly IPrestadorService _prestadorService;

    public AvaliacaoService(IAvaliacaoRepository avaliacaoRepository, IPrestadorService prestadorService)
    {
        _avaliacaoRepository = avaliacaoRepository;
        _prestadorService = prestadorService;
    }

    public async Task<AvaliacaoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var avaliacao = await _avaliacaoRepository.ObterPorIdAsync(id, cancellationToken);
        return avaliacao?.ParaDto();
    }

    public async Task<AvaliacaoDto?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        var avaliacao = await _avaliacaoRepository.ObterPorAgendamentoAsync(agendamentoId, cancellationToken);
        return avaliacao?.ParaDto();
    }

    public async Task<AvaliacaoDto> CriarAsync(AvaliacaoDto dto, CancellationToken cancellationToken = default)
    {
        var podeAvaliar = await _avaliacaoRepository.PodeAvaliarAsync(dto.ClienteId, dto.AgendamentoId, cancellationToken);
        if (!podeAvaliar)
            throw new InvalidOperationException("Somente serviços concluídos podem ser avaliados");

        var existente = await _avaliacaoRepository.ObterPorAgendamentoAsync(dto.AgendamentoId, cancellationToken);
        if (existente != null)
            throw new InvalidOperationException("Este serviço já foi avaliado");

        var avaliacao = dto.ParaEntidade();
        avaliacao.Publicar(DateTime.UtcNow);

        await _avaliacaoRepository.AdicionarAsync(avaliacao, cancellationToken);
        await _avaliacaoRepository.SalvarAlteracoesAsync(cancellationToken);
        await _prestadorService.AtualizarMediaAvaliacoesAsync(dto.PrestadorId, cancellationToken);
        return avaliacao.ParaDto();
    }

    public async Task<IEnumerable<AvaliacaoDto>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        var avaliacoes = await _avaliacaoRepository.ObterPorPrestadorAsync(prestadorId, cancellationToken);
        return avaliacoes.Select(a => a.ParaDto());
    }

    public async Task<IEnumerable<AvaliacaoDto>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var avaliacoes = await _avaliacaoRepository.ObterPorClienteAsync(clienteId, cancellationToken);
        return avaliacoes.Select(a => a.ParaDto());
    }

    public Task<bool> PodeAvaliarAsync(Guid clienteId, Guid agendamentoId, CancellationToken cancellationToken = default) =>
        _avaliacaoRepository.PodeAvaliarAsync(clienteId, agendamentoId, cancellationToken);
}

public class PagamentoService : IPagamentoService
{
    private readonly IPagamentoRepository _pagamentoRepository;

    public PagamentoService(IPagamentoRepository pagamentoRepository)
    {
        _pagamentoRepository = pagamentoRepository;
    }

    public async Task<PagamentoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pagamento = await _pagamentoRepository.ObterPorIdAsync(id, cancellationToken);
        return pagamento?.ParaDto();
    }

    public async Task<PagamentoDto?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        var pagamento = await _pagamentoRepository.ObterPorAgendamentoAsync(agendamentoId, cancellationToken);
        return pagamento?.ParaDto();
    }

    public async Task<PagamentoDto> CriarAsync(PagamentoDto dto, CancellationToken cancellationToken = default)
    {
        var pagamento = dto.ParaEntidade();
        pagamento.DefinirComoPendente(DateTime.UtcNow);
        await _pagamentoRepository.AdicionarAsync(pagamento, cancellationToken);
        await _pagamentoRepository.SalvarAlteracoesAsync(cancellationToken);
        return pagamento.ParaDto();
    }

    public async Task<PagamentoDto> ProcessarAsync(Guid pagamentoId, CancellationToken cancellationToken = default)
    {
        var pagamento = await ObterPagamentoOuFalhar(pagamentoId, cancellationToken);
        if (pagamento.Status != StatusPagamento.Pendente)
            throw new InvalidOperationException("Pagamento não pode ser processado neste status");

        pagamento.Processar(DateTime.UtcNow);
        _pagamentoRepository.Atualizar(pagamento);
        await _pagamentoRepository.SalvarAlteracoesAsync(cancellationToken);
        return pagamento.ParaDto();
    }

    public async Task<PagamentoDto> ConfirmarAsync(Guid pagamentoId, string transacaoId, CancellationToken cancellationToken = default)
    {
        var pagamento = await ObterPagamentoOuFalhar(pagamentoId, cancellationToken);
        if (pagamento.Status != StatusPagamento.Processando)
            throw new InvalidOperationException("Pagamento não pode ser confirmado neste status");

        pagamento.Aprovar(transacaoId, DateTime.UtcNow);
        _pagamentoRepository.Atualizar(pagamento);
        await _pagamentoRepository.SalvarAlteracoesAsync(cancellationToken);
        return pagamento.ParaDto();
    }

    public async Task<PagamentoDto> RecusarAsync(Guid pagamentoId, string motivo, CancellationToken cancellationToken = default)
    {
        var pagamento = await ObterPagamentoOuFalhar(pagamentoId, cancellationToken);
        if (pagamento.Status != StatusPagamento.Processando)
            throw new InvalidOperationException("Pagamento não pode ser recusado neste status");

        pagamento.Recusar(motivo);
        _pagamentoRepository.Atualizar(pagamento);
        await _pagamentoRepository.SalvarAlteracoesAsync(cancellationToken);
        return pagamento.ParaDto();
    }

    public async Task<PagamentoDto> EstornarAsync(Guid pagamentoId, CancellationToken cancellationToken = default)
    {
        var pagamento = await ObterPagamentoOuFalhar(pagamentoId, cancellationToken);
        if (pagamento.Status != StatusPagamento.Aprovado)
            throw new InvalidOperationException("Pagamento não pode ser estornado neste status");

        pagamento.Estornar();
        _pagamentoRepository.Atualizar(pagamento);
        await _pagamentoRepository.SalvarAlteracoesAsync(cancellationToken);
        return pagamento.ParaDto();
    }

    private async Task<Domain.Entidades.Pagamento> ObterPagamentoOuFalhar(Guid id, CancellationToken cancellationToken)
    {
        var pagamento = await _pagamentoRepository.ObterPorIdAsync(id, cancellationToken);
        return pagamento ?? throw new InvalidOperationException("Pagamento não encontrado");
    }
}

public class MensagemService : IMensagemService
{
    private readonly IMensagemRepository _mensagemRepository;

    public MensagemService(IMensagemRepository mensagemRepository)
    {
        _mensagemRepository = mensagemRepository;
    }

    public async Task<MensagemDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var mensagem = await _mensagemRepository.ObterPorIdAsync(id, cancellationToken);
        return mensagem?.ParaDto();
    }

    public async Task<MensagemDto> EnviarAsync(MensagemDto dto, CancellationToken cancellationToken = default)
    {
        var mensagem = dto.ParaEntidade();
        mensagem.PrepararEnvio(DateTime.UtcNow);
        await _mensagemRepository.AdicionarAsync(mensagem, cancellationToken);
        await _mensagemRepository.SalvarAlteracoesAsync(cancellationToken);
        return mensagem.ParaDto();
    }

    public async Task<IEnumerable<MensagemDto>> ObterConversaAsync(Guid conversaId, CancellationToken cancellationToken = default)
    {
        var mensagens = await _mensagemRepository.ObterConversaAsync(conversaId, cancellationToken);
        return mensagens.Select(m => m.ParaDto());
    }

    public async Task<IEnumerable<MensagemDto>> ObterConversasPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var mensagens = await _mensagemRepository.ObterConversasPorUsuarioAsync(usuarioId, cancellationToken);
        return mensagens.Select(m => m.ParaDto());
    }

    public async Task MarcarComoLidaAsync(Guid mensagemId, CancellationToken cancellationToken = default)
    {
        var mensagem = await _mensagemRepository.ObterPorIdAsync(mensagemId, cancellationToken);
        if (mensagem == null || mensagem.Lida)
            return;

        mensagem.MarcarComoLida(DateTime.UtcNow);
        _mensagemRepository.Atualizar(mensagem);
        await _mensagemRepository.SalvarAlteracoesAsync(cancellationToken);
    }

    public Task<int> ObterNaoLidasAsync(Guid usuarioId, CancellationToken cancellationToken = default) =>
        _mensagemRepository.ObterNaoLidasAsync(usuarioId, cancellationToken);
}
