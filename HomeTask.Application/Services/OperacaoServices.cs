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
            throw new InvalidOperationException("Nenhum servico valido encontrado para o agendamento.");

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
            throw new InvalidOperationException("Cliente nao possui endereco cadastrado para o agendamento.");

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
            throw new InvalidOperationException("Agendamento nao pode ser aceito neste status");

        agendamento.Aceitar(DateTime.UtcNow);
        _agendamentoRepository.Atualizar(agendamento);
        await _agendamentoRepository.SalvarAlteracoesAsync(cancellationToken);
        return agendamento.ParaDto();
    }

    public async Task<AgendamentoDto> RecusarAsync(Guid agendamentoId, string motivo, CancellationToken cancellationToken = default)
    {
        var agendamento = await ObterAgendamentoOuFalhar(agendamentoId, cancellationToken);
        if (agendamento.Status != StatusAgendamento.Solicitado)
            throw new InvalidOperationException("Agendamento nao pode ser recusado neste status");

        agendamento.Recusar(motivo, DateTime.UtcNow);
        _agendamentoRepository.Atualizar(agendamento);
        await _agendamentoRepository.SalvarAlteracoesAsync(cancellationToken);
        return agendamento.ParaDto();
    }

    public async Task<AgendamentoDto> IniciarAsync(Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        var agendamento = await ObterAgendamentoOuFalhar(agendamentoId, cancellationToken);
        if (agendamento.Status != StatusAgendamento.Aceito)
            throw new InvalidOperationException("Agendamento nao pode ser iniciado neste status");

        agendamento.Iniciar(DateTime.UtcNow);
        _agendamentoRepository.Atualizar(agendamento);
        await _agendamentoRepository.SalvarAlteracoesAsync(cancellationToken);
        return agendamento.ParaDto();
    }

    public async Task<AgendamentoDto> ConcluirAsync(Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        var agendamento = await ObterAgendamentoOuFalhar(agendamentoId, cancellationToken);
        if (agendamento.Status != StatusAgendamento.EmAndamento)
            throw new InvalidOperationException("Agendamento nao pode ser concluido neste status");

        var dataConclusao = DateTime.UtcNow;
        var valorFinal = CalcularValorFinal(agendamento, dataConclusao);
        agendamento.MarcarAguardandoPagamento(dataConclusao, valorFinal);
        _agendamentoRepository.Atualizar(agendamento);
        await _agendamentoRepository.SalvarAlteracoesAsync(cancellationToken);
        return agendamento.ParaDto();
    }

    public async Task<AgendamentoDto> CancelarAsync(Guid agendamentoId, string motivo, CancellationToken cancellationToken = default)
    {
        var agendamento = await ObterAgendamentoOuFalhar(agendamentoId, cancellationToken);
        if (agendamento.Status is StatusAgendamento.Concluido or StatusAgendamento.Cancelado)
            throw new InvalidOperationException("Agendamento nao pode ser cancelado neste status");

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
        return agendamento ?? throw new InvalidOperationException("Agendamento nao encontrado");
    }

    private static decimal CalcularValorFinal(Domain.Entidades.Agendamento agendamento, DateTime dataConclusao)
    {
        if (agendamento.AgendamentoServicos.Count == 0)
            return agendamento.ValorTotal;

        var horasCobradas = ObterHorasCobradas(agendamento, dataConclusao);
        decimal valorFinal = 0;

        foreach (var item in agendamento.AgendamentoServicos)
        {
            var servico = item.ServicoBase;
            if (servico == null)
            {
                valorFinal += item.Quantidade * item.ValorUnitario;
                continue;
            }

            if (servico.UnidadeCobranca == FormatoCobranca.PorHora)
            {
                item.AtualizarCobranca(horasCobradas, servico.PrecoBase);
            }
            else
            {
                item.AtualizarCobranca(1, servico.PrecoBase);
            }

            valorFinal += item.Quantidade * item.ValorUnitario;
        }

        return valorFinal;
    }

    private static int ObterHorasCobradas(Domain.Entidades.Agendamento agendamento, DateTime dataConclusao)
    {
        var dataInicio = agendamento.DataInicio ?? dataConclusao;
        var duracao = dataConclusao - dataInicio;
        if (duracao <= TimeSpan.Zero)
            return 1;

        return Math.Max(1, (int)Math.Ceiling(duracao.TotalHours));
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
            throw new InvalidOperationException("Somente servicos concluidos podem ser avaliados");

        var existente = await _avaliacaoRepository.ObterPorAgendamentoAsync(dto.AgendamentoId, cancellationToken);
        if (existente != null)
            throw new InvalidOperationException("Este servico ja foi avaliado");

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
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IPrestadorRepository _prestadorRepository;
    private readonly IPagamentoGateway _pagamentoGateway;

    public PagamentoService(
        IPagamentoRepository pagamentoRepository,
        IAgendamentoRepository agendamentoRepository,
        IPrestadorRepository prestadorRepository,
        IPagamentoGateway pagamentoGateway)
    {
        _pagamentoRepository = pagamentoRepository;
        _agendamentoRepository = agendamentoRepository;
        _prestadorRepository = prestadorRepository;
        _pagamentoGateway = pagamentoGateway;
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

    public async Task<PagamentoDto> IniciarCheckoutAsync(Guid agendamentoId, Guid clienteId, CancellationToken cancellationToken = default)
    {
        var agendamento = await ObterAgendamentoOuFalhar(agendamentoId, cancellationToken);
        if (agendamento.ClienteId != clienteId)
            throw new InvalidOperationException("Cliente nao pode iniciar pagamento deste agendamento");
        if (agendamento.Status != StatusAgendamento.AguardandoPagamento)
            throw new InvalidOperationException("Agendamento nao esta aguardando pagamento");

        var pagamento = await _pagamentoRepository.ObterPorAgendamentoAsync(agendamentoId, cancellationToken);
        var pagamentoCriadoAgora = false;
        if (pagamento == null)
        {
            pagamento = new Domain.Entidades.Pagamento();
            pagamento.DefinirDados(
                Guid.NewGuid(),
                agendamento.Id,
                agendamento.ValorTotal,
                TipoPagamento.Pix,
                StatusPagamento.Pendente,
                null,
                null,
                null,
                null,
                null,
                DateTime.UtcNow,
                null,
                null,
                null);
            await _pagamentoRepository.AdicionarAsync(pagamento, cancellationToken);
            pagamentoCriadoAgora = true;
        }
        else if (pagamento.Valor != agendamento.ValorTotal)
        {
            pagamento.AtualizarValor(agendamento.ValorTotal);
        }

        var precisaGerarNovoCheckout = pagamento.Status != StatusPagamento.Aprovado;

        if (precisaGerarNovoCheckout)
        {
            var checkout = await _pagamentoGateway.CriarCheckoutPixAsync(
                new PagamentoCheckoutRequestDto
                {
                    PagamentoId = pagamento.Id,
                    AgendamentoId = agendamento.Id,
                    Valor = pagamento.Valor,
                    Descricao = $"Agendamento {agendamento.Id}",
                    ClienteEmail = agendamento.Cliente.Usuario.Email,
                    ClienteNome = agendamento.Cliente.Usuario.Nome,
                    ClienteDocumento = agendamento.Cliente.Usuario.Documento
                },
                cancellationToken);

            if (pagamento.Status == StatusPagamento.Pendente)
                pagamento.Processar(DateTime.UtcNow);

            pagamento.RegistrarCheckout(
                checkout.CheckoutExternoId,
                checkout.CheckoutUrl,
                checkout.StatusExterno,
                checkout.PayloadExterno);
            if (!pagamentoCriadoAgora)
                _pagamentoRepository.Atualizar(pagamento);
        }

        await _pagamentoRepository.SalvarAlteracoesAsync(cancellationToken);
        return pagamento.ParaDto();
    }

    public async Task<PagamentoDto?> ProcessarWebhookAsync(PagamentoWebhookDto webhook, CancellationToken cancellationToken = default)
    {
        if (!string.Equals(webhook.Topico, "payment", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(webhook.PagamentoExternoId))
            return null;

        return await ReconciliarPagamentoInternoAsync(
            webhook.PagamentoExternoId,
            webhook.PayloadExterno,
            cancellationToken);
    }

    public async Task<PagamentoDto?> ReconciliarPagamentoExternoAsync(string pagamentoExternoId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(pagamentoExternoId))
            return null;

        return await ReconciliarPagamentoInternoAsync(pagamentoExternoId, null, cancellationToken);
    }

    private async Task<Domain.Entidades.Agendamento> ObterAgendamentoOuFalhar(Guid id, CancellationToken cancellationToken)
    {
        var agendamento = await _agendamentoRepository.ObterPorIdAsync(id, cancellationToken);
        return agendamento ?? throw new InvalidOperationException("Agendamento nao encontrado");
    }

    private async Task<Domain.Entidades.Pagamento> ObterPagamentoOuFalhar(Guid id, CancellationToken cancellationToken)
    {
        var pagamento = await _pagamentoRepository.ObterPorIdAsync(id, cancellationToken);
        return pagamento ?? throw new InvalidOperationException("Pagamento nao encontrado");
    }

    private async Task<PagamentoDto?> ReconciliarPagamentoInternoAsync(
        string pagamentoExternoId,
        string? payloadExterno,
        CancellationToken cancellationToken)
    {
        var statusGateway = await _pagamentoGateway.ObterStatusPagamentoAsync(pagamentoExternoId, cancellationToken);
        if (statusGateway == null || !Guid.TryParse(statusGateway.ReferenciaInterna, out var pagamentoId))
            return null;

        var pagamento = await ObterPagamentoOuFalhar(pagamentoId, cancellationToken);
        pagamento.AtualizarRetornoGateway(
            statusGateway.PagamentoExternoId,
            statusGateway.StatusExterno,
            statusGateway.PayloadExterno ?? payloadExterno,
            statusGateway.MotivoRecusa);

        if (statusGateway.Status == StatusPagamento.Aprovado && pagamento.Status != StatusPagamento.Aprovado)
        {
            pagamento.Aprovar(statusGateway.PagamentoExternoId, DateTime.UtcNow);

            var agendamento = await ObterAgendamentoOuFalhar(pagamento.AgendamentoId, cancellationToken);
            if (agendamento.Status != StatusAgendamento.Concluido)
            {
                agendamento.ConcluirFinanceiramente();
                _agendamentoRepository.Atualizar(agendamento);

                var prestador = await _prestadorRepository.ObterPorIdAsync(agendamento.PrestadorId, cancellationToken);
                if (prestador != null)
                {
                    prestador.IncrementarTotalServicosConcluidos();
                    _prestadorRepository.Atualizar(prestador);
                }
            }
        }
        else if (statusGateway.Status == StatusPagamento.Recusado && pagamento.Status != StatusPagamento.Aprovado)
        {
            pagamento.Recusar(statusGateway.MotivoRecusa ?? "Pagamento recusado pelo gateway");
        }
        else if (statusGateway.Status is StatusPagamento.Processando or StatusPagamento.Pendente &&
            pagamento.Status == StatusPagamento.Pendente)
        {
            pagamento.Processar(DateTime.UtcNow);
        }

        _pagamentoRepository.Atualizar(pagamento);
        await _pagamentoRepository.SalvarAlteracoesAsync(cancellationToken);
        return pagamento.ParaDto();
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
