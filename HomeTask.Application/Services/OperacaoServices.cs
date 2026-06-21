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
    private readonly IServicoPrestadorRepository _servicoPrestadorRepository;

    public AgendamentoService(
        IAgendamentoRepository agendamentoRepository,
        IPrestadorRepository prestadorRepository,
        IServicoPrestadorRepository servicoPrestadorRepository)
    {
        _agendamentoRepository = agendamentoRepository;
        _prestadorRepository = prestadorRepository;
        _servicoPrestadorRepository = servicoPrestadorRepository;
    }

    public async Task<AgendamentoResumoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var agendamento = await _agendamentoRepository.ObterPorIdAsync(id, cancellationToken);
        return agendamento?.ParaResumoDto();
    }

    public async Task<AgendamentoDto> CriarAsync(AgendamentoDto dto, CancellationToken cancellationToken = default)
    {
        var agendamento = dto.ParaEntidade();
        agendamento.DefinirEnderecoDescricao(dto.EnderecoDescricao);
        var servicosIds = dto.ServicosOferecidosIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        if (dto.PrincipalServicoPrestadorId.HasValue && dto.PrincipalServicoPrestadorId.Value != Guid.Empty && !servicosIds.Contains(dto.PrincipalServicoPrestadorId.Value))
            servicosIds.Add(dto.PrincipalServicoPrestadorId.Value);

        var servicos = await _agendamentoRepository.ObterServicosPorIdsAsync(servicosIds, cancellationToken);

        if (servicos.Count == 0)
            throw new InvalidOperationException("Nenhum servico valido encontrado para o agendamento.");

        decimal valorTotal = 0;
        var duracaoTotal = 0;
        var prestadorId = agendamento.PrestadorId;
        var clienteId = agendamento.ClienteId;
        Guid? principalServicoPrestadorId = dto.PrincipalServicoPrestadorId;
        Domain.Entidades.ServicoCliente? servicoClienteDoPedido = null;
        Domain.Entidades.ServicoPrestador? servicoPrestadorPrincipalResolvido = null;

        foreach (var servico in servicos)
        {
            if (servico is Domain.Entidades.ServicoPrestador sp)
            {
                if (prestadorId == Guid.Empty)
                    prestadorId = sp.PrestadorId;

                principalServicoPrestadorId ??= sp.Id;

                duracaoTotal += sp.DuracaoEstimadaMinutos ?? 0;
            }

            decimal valorUnitario = servico.PrecoBase;

            if (servico is Domain.Entidades.ServicoCliente sc)
            {
                servicoClienteDoPedido ??= sc;

                if (clienteId == Guid.Empty)
                    clienteId = sc.ClienteId;

                if (sc.UnidadeCobranca == FormatoCobranca.ACombinar)
                {
                    if (!dto.ValorProposto.HasValue || dto.ValorProposto.Value <= 0)
                        throw new InvalidOperationException("Informe um valor de proposta para pedidos com valor a combinar.");

                    valorUnitario = dto.ValorProposto.Value;
                }
            }

            var agendamentoServico = new Domain.Entidades.AgendamentoServico();
            agendamentoServico.DefinirDados(agendamento.Id, servico.Id, 1, valorUnitario);
            agendamento.AdicionarServico(agendamentoServico);
            valorTotal += valorUnitario;
        }

        if (!principalServicoPrestadorId.HasValue && prestadorId != Guid.Empty && servicoClienteDoPedido != null)
        {
            var servicoPrestadorPrincipal = await ResolverServicoPrestadorPrincipalAsync(
                prestadorId,
                servicoClienteDoPedido.Categoria,
                cancellationToken);

            if (servicoPrestadorPrincipal != null)
            {
                servicoPrestadorPrincipalResolvido = servicoPrestadorPrincipal;
                principalServicoPrestadorId = servicoPrestadorPrincipal.Id;

                if (duracaoTotal == 0)
                    duracaoTotal = servicoPrestadorPrincipal.DuracaoEstimadaMinutos ?? 0;
            }
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

        if (principalServicoPrestadorId.HasValue)
        {
            var servicoPrestadorPrincipal = servicoPrestadorPrincipalResolvido;
            if (servicoPrestadorPrincipal == null)
            {
                var servicoPrincipal = await _agendamentoRepository.ObterServicosPorIdsAsync([principalServicoPrestadorId.Value], cancellationToken);
                servicoPrestadorPrincipal = servicoPrincipal.OfType<Domain.Entidades.ServicoPrestador>().FirstOrDefault();
            }

            if (servicoPrestadorPrincipal == null || servicoPrestadorPrincipal.PrestadorId != prestadorId)
                throw new InvalidOperationException("O servico principal do prestador informado nao pertence ao agendamento.");
        }

        agendamento.DefinirDados(
            agendamento.Id,
            clienteId,
            prestadorId,
            principalServicoPrestadorId,
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
            else if (servico.UnidadeCobranca == FormatoCobranca.ACombinar)
            {
                item.AtualizarCobranca(1, item.ValorUnitario);
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

    private async Task<Domain.Entidades.ServicoPrestador?> ResolverServicoPrestadorPrincipalAsync(
        Guid prestadorId,
        CategoriaServico categoria,
        CancellationToken cancellationToken)
    {
        var servicosDoPrestador = await _servicoPrestadorRepository.ObterPorPrestadorAsync(prestadorId, cancellationToken);

        return servicosDoPrestador.FirstOrDefault(servico => servico.Ativo && servico.Categoria == categoria)
            ?? servicosDoPrestador.FirstOrDefault(servico => servico.Ativo)
            ?? servicosDoPrestador.FirstOrDefault();
    }
}

public class AvaliacaoService : IAvaliacaoService
{
    private readonly IAvaliacaoRepository _avaliacaoRepository;
    private readonly IPrestadorService _prestadorService;
    private readonly IServicoPrestadorService _servicoPrestadorService;

    public AvaliacaoService(
        IAvaliacaoRepository avaliacaoRepository,
        IPrestadorService prestadorService,
        IServicoPrestadorService servicoPrestadorService)
    {
        _avaliacaoRepository = avaliacaoRepository;
        _prestadorService = prestadorService;
        _servicoPrestadorService = servicoPrestadorService;
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
        var agendamento = await _avaliacaoRepository.ObterAgendamentoElegivelParaAvaliacaoAsync(dto.ClienteId, dto.AgendamentoId, cancellationToken);
        if (agendamento == null)
            throw new InvalidOperationException("Somente servicos concluidos com pagamento aprovado podem ser avaliados");

        var existente = await _avaliacaoRepository.ObterPorAgendamentoAsync(dto.AgendamentoId, cancellationToken);
        if (existente != null)
            throw new InvalidOperationException("Este servico ja foi avaliado");

        dto.PrestadorId = agendamento.PrestadorId;
        dto.ServicoPrestadorId = await ResolverServicoPrestadorAvaliadoAsync(agendamento, cancellationToken);

        if (dto.ServicoPrestadorId == Guid.Empty)
            throw new InvalidOperationException("Nao foi possivel identificar o servico principal do prestador neste agendamento");

        var avaliacao = dto.ParaEntidade();
        avaliacao.Publicar(DateTime.UtcNow);

        await _avaliacaoRepository.AdicionarAsync(avaliacao, cancellationToken);
        await _avaliacaoRepository.SalvarAlteracoesAsync(cancellationToken);
        await _prestadorService.AtualizarMediaAvaliacoesAsync(dto.PrestadorId, cancellationToken);
        await _servicoPrestadorService.AtualizarMediaAvaliacoesAsync(dto.ServicoPrestadorId, cancellationToken);
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

    private async Task<Guid> ResolverServicoPrestadorAvaliadoAsync(
        Domain.Entidades.Agendamento agendamento,
        CancellationToken cancellationToken)
    {
        if (agendamento.PrincipalServicoPrestadorId.HasValue)
            return agendamento.PrincipalServicoPrestadorId.Value;

        var servicoPrestadorNoAgendamento = agendamento.AgendamentoServicos
            .Select(item => item.ServicoBase)
            .OfType<Domain.Entidades.ServicoPrestador>()
            .FirstOrDefault();

        if (servicoPrestadorNoAgendamento != null)
            return servicoPrestadorNoAgendamento.Id;

        var categoriaDoPedido = agendamento.AgendamentoServicos
            .Select(item => item.ServicoBase)
            .OfType<Domain.Entidades.ServicoCliente>()
            .Select(servico => servico.Categoria)
            .FirstOrDefault();

        if (categoriaDoPedido == default)
            return Guid.Empty;

        var servicosDoPrestador = await _servicoPrestadorService.ObterPorPrestadorAsync(agendamento.PrestadorId, cancellationToken);
        return servicosDoPrestador
            .Where(servico => servico.Categoria == categoriaDoPedido)
            .Select(servico => servico.Id)
            .FirstOrDefault();
    }
}

public class AvaliacaoClienteService : IAvaliacaoClienteService
{
    private readonly IAvaliacaoClienteRepository _avaliacaoClienteRepository;
    private readonly IClienteService _clienteService;

    public AvaliacaoClienteService(
        IAvaliacaoClienteRepository avaliacaoClienteRepository,
        IClienteService clienteService)
    {
        _avaliacaoClienteRepository = avaliacaoClienteRepository;
        _clienteService = clienteService;
    }

    public async Task<AvaliacaoClienteDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var avaliacao = await _avaliacaoClienteRepository.ObterPorIdAsync(id, cancellationToken);
        return avaliacao?.ParaDto();
    }

    public async Task<AvaliacaoClienteDto?> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        var avaliacao = await _avaliacaoClienteRepository.ObterPorAgendamentoAsync(agendamentoId, cancellationToken);
        return avaliacao?.ParaDto();
    }

    public async Task<AvaliacaoClienteDto> CriarAsync(AvaliacaoClienteDto dto, CancellationToken cancellationToken = default)
    {
        var agendamento = await _avaliacaoClienteRepository.ObterAgendamentoElegivelParaAvaliacaoAsync(dto.PrestadorId, dto.AgendamentoId, cancellationToken);
        if (agendamento == null)
            throw new InvalidOperationException("Somente servicos concluidos com pagamento aprovado podem ser avaliados");

        var existente = await _avaliacaoClienteRepository.ObterPorAgendamentoAsync(dto.AgendamentoId, cancellationToken);
        if (existente != null)
            throw new InvalidOperationException("Este cliente ja foi avaliado neste agendamento");

        dto.ClienteId = agendamento.ClienteId;

        var avaliacao = dto.ParaEntidade();
        avaliacao.Publicar(DateTime.UtcNow);

        await _avaliacaoClienteRepository.AdicionarAsync(avaliacao, cancellationToken);
        await _avaliacaoClienteRepository.SalvarAlteracoesAsync(cancellationToken);
        await _clienteService.AtualizarMediaAvaliacoesAsync(dto.ClienteId, cancellationToken);
        return avaliacao.ParaDto();
    }

    public async Task<IEnumerable<AvaliacaoClienteDto>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var avaliacoes = await _avaliacaoClienteRepository.ObterPorClienteAsync(clienteId, cancellationToken);
        return avaliacoes.Select(a => a.ParaDto());
    }

    public async Task<IEnumerable<AvaliacaoClienteDto>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        var avaliacoes = await _avaliacaoClienteRepository.ObterPorPrestadorAsync(prestadorId, cancellationToken);
        return avaliacoes.Select(a => a.ParaDto());
    }

    public Task<bool> PodeAvaliarAsync(Guid prestadorId, Guid agendamentoId, CancellationToken cancellationToken = default) =>
        _avaliacaoClienteRepository.PodeAvaliarAsync(prestadorId, agendamentoId, cancellationToken);
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
    private const int LimiteMensagensConsecutivas = 3;
    private static readonly TimeSpan RetencaoMensagens = TimeSpan.FromDays(30);
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

    public async Task<MensagemDto> EnviarPorAgendamentoAsync(Guid agendamentoId, Guid remetenteId, string conteudo, CancellationToken cancellationToken = default)
    {
        var texto = conteudo.Trim();
        if (string.IsNullOrWhiteSpace(texto))
            throw new InvalidOperationException("A mensagem nao pode estar vazia.");

        if (texto.Length > 2000)
            throw new InvalidOperationException("A mensagem deve ter no maximo 2000 caracteres.");

        var ultimasMensagens = await _mensagemRepository.ObterUltimasPorAgendamentoAsync(
            agendamentoId,
            LimiteMensagensConsecutivas,
            cancellationToken);

        if (ultimasMensagens.Count == LimiteMensagensConsecutivas && ultimasMensagens.All(m => m.RemetenteId == remetenteId))
            throw new InvalidOperationException("Aguarde a resposta do outro participante antes de enviar novas mensagens.");

        return await EnviarAsync(new MensagemDto
        {
            RemetenteId = remetenteId,
            AgendamentoId = agendamentoId,
            Conteudo = texto
        }, cancellationToken);
    }

    public async Task<IEnumerable<MensagemDto>> ObterConversaAsync(Guid conversaId, CancellationToken cancellationToken = default)
    {
        var mensagens = await _mensagemRepository.ObterConversaAsync(conversaId, cancellationToken);
        return mensagens.Select(m => m.ParaDto());
    }

    public async Task<IEnumerable<MensagemDto>> ObterPorAgendamentoAsync(Guid agendamentoId, CancellationToken cancellationToken = default)
    {
        var desde = DateTime.UtcNow.Subtract(RetencaoMensagens);
        var mensagens = await _mensagemRepository.ObterPorAgendamentoAsync(agendamentoId, desde, cancellationToken);
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

    public Task<int> RemoverExpiradasAsync(CancellationToken cancellationToken = default) =>
        _mensagemRepository.RemoverEnviadasAntesDeAsync(DateTime.UtcNow.Subtract(RetencaoMensagens), cancellationToken);
}
