using HomeTask.Application.Dtos;
using HomeTask.Application.Interfaces;
using HomeTask.Application.Mappings;
using HomeTask.Domain.Enums;
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
}

public class PrestadorService : IPrestadorService
{
    private readonly IPrestadorRepository _prestadorRepository;

    public PrestadorService(IPrestadorRepository prestadorRepository)
    {
        _prestadorRepository = prestadorRepository;
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

    public async Task AtualizarMediaAvaliacoesAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        var prestador = await _prestadorRepository.ObterComAvaliacoesAsync(prestadorId, cancellationToken);
        if (prestador == null || prestador.Avaliacoes.Count == 0)
            return;

        prestador.AtualizarMetricasAvaliacao(
            (decimal)prestador.Avaliacoes.Average(a => a.Nota),
            prestador.Avaliacoes.Count);

        var avaliacoesRecentes = prestador.Avaliacoes
            .OrderByDescending(a => a.DataAvaliacao)
            .Take(5)
            .ToList();

        if (avaliacoesRecentes.Count >= 5 && avaliacoesRecentes.Average(a => a.Nota) < 2)
            prestador.DefinirStatus(StatusPrestador.Suspenso);

        _prestadorRepository.Atualizar(prestador);
        await _prestadorRepository.SalvarAlteracoesAsync(cancellationToken);
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
