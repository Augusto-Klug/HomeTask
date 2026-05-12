using HomeTask.Application.Dtos;
using HomeTask.Application.Interfaces;
using HomeTask.Application.Mappings;
using HomeTask.Domain.Repositories;

namespace HomeTask.Application.Services;

public class ServicoPrestadorService : IServicoPrestadorService
{
    private readonly IServicoPrestadorRepository _servicoPrestadorRepository;

    public ServicoPrestadorService(IServicoPrestadorRepository servicoPrestadorRepository)
    {
        _servicoPrestadorRepository = servicoPrestadorRepository;
    }

    public async Task<ServicoPrestadorDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var servico = await _servicoPrestadorRepository.ObterPorIdAsync(id, cancellationToken);
        return servico?.ParaDto();
    }

    public async Task<ServicoPrestadorDto> CriarAsync(ServicoPrestadorDto dto, CancellationToken cancellationToken = default)
    {
        var servico = dto.ParaEntidade();
        await _servicoPrestadorRepository.AdicionarAsync(servico, cancellationToken);
        await _servicoPrestadorRepository.SalvarAlteracoesAsync(cancellationToken);
        return servico.ParaDto();
    }

    public async Task<ServicoPrestadorDto> AtualizarAsync(ServicoPrestadorDto dto, CancellationToken cancellationToken = default)
    {
        var servico = dto.ParaEntidade();
        _servicoPrestadorRepository.Atualizar(servico);
        await _servicoPrestadorRepository.SalvarAlteracoesAsync(cancellationToken);
        return servico.ParaDto();
    }

    public async Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var servico = await _servicoPrestadorRepository.ObterPorIdAsync(id, cancellationToken);
        if (servico == null)
            return false;

        servico.Desativar();
        _servicoPrestadorRepository.Atualizar(servico);
        await _servicoPrestadorRepository.SalvarAlteracoesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<ServicoPrestadorDto>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        var servicos = await _servicoPrestadorRepository.ObterPorPrestadorAsync(prestadorId, cancellationToken);
        return servicos.Select(s => s.ParaDto());
    }

    public async Task<IEnumerable<ServicoPrestadorDto>> BuscarAsync(HomeTask.Domain.Enums.CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken = default)
    {
        var servicos = await _servicoPrestadorRepository.BuscarAsync(categoria, cidade, precoMaximo, cancellationToken);
        return servicos.Select(s => s.ParaDto());
    }

    public async Task<ServicoBuscaPaginadaDto> BuscarTodosPaginadoAsync(HomeTask.Domain.Enums.CategoriaServico? categoria, string? cidade, decimal? precoMaximo, Guid? usuarioId, int pagina, int tamanhoPagina, CancellationToken cancellationToken = default)
    {
        var resultado = await _servicoPrestadorRepository.BuscarTodosPaginadoAsync(categoria, cidade, precoMaximo, usuarioId, pagina, tamanhoPagina, cancellationToken);
        return resultado.ParaDto();
    }
}

public class ServicoClienteService : IServicoClienteService
{
    private readonly IServicoClienteRepository _servicoClienteRepository;

    public ServicoClienteService(IServicoClienteRepository servicoClienteRepository)
    {
        _servicoClienteRepository = servicoClienteRepository;
    }

    public async Task<ServicoClienteDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var servico = await _servicoClienteRepository.ObterPorIdAsync(id, cancellationToken);
        return servico?.ParaDto();
    }

    public async Task<ServicoClienteDto> CriarAsync(ServicoClienteDto dto, CancellationToken cancellationToken = default)
    {
        var servico = dto.ParaEntidade();
        await _servicoClienteRepository.AdicionarAsync(servico, cancellationToken);
        await _servicoClienteRepository.SalvarAlteracoesAsync(cancellationToken);
        return servico.ParaDto();
    }

    public async Task<ServicoClienteDto> AtualizarAsync(ServicoClienteDto dto, CancellationToken cancellationToken = default)
    {
        var servico = dto.ParaEntidade();
        _servicoClienteRepository.Atualizar(servico);
        await _servicoClienteRepository.SalvarAlteracoesAsync(cancellationToken);
        return servico.ParaDto();
    }

    public async Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var servico = await _servicoClienteRepository.ObterPorIdAsync(id, cancellationToken);
        if (servico == null)
            return false;

        servico.Desativar();
        _servicoClienteRepository.Atualizar(servico);
        await _servicoClienteRepository.SalvarAlteracoesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<ServicoClienteDto>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var servicos = await _servicoClienteRepository.ObterPorClienteAsync(clienteId, cancellationToken);
        return servicos.Select(s => s.ParaDto());
    }

    public async Task<IEnumerable<ServicoClienteDto>> BuscarPedidosAsync(HomeTask.Domain.Enums.CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken = default)
    {
        var servicos = await _servicoClienteRepository.BuscarPedidosAsync(categoria, cidade, precoMaximo, cancellationToken);
        return servicos.Select(s => s.ParaDto());
    }
}

public class ConversaService : IConversaService
{
    private readonly IConversaRepository _conversaRepository;

    public ConversaService(IConversaRepository conversaRepository)
    {
        _conversaRepository = conversaRepository;
    }

    public async Task<ConversaDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var conversa = await _conversaRepository.ObterPorIdAsync(id, cancellationToken);
        return conversa?.ParaDto();
    }

    public async Task<IEnumerable<ConversaDto>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var conversas = await _conversaRepository.ListarPorUsuarioAsync(usuarioId, cancellationToken);
        return conversas.Select(c => c.ParaDto());
    }

    public async Task<ConversaDto> ObterOuCriarAsync(Guid clienteId, Guid prestadorId, CancellationToken cancellationToken = default)
    {
        var conversa = await _conversaRepository.ObterPorClientePrestadorAsync(clienteId, prestadorId, cancellationToken);
        if (conversa == null)
        {
            conversa = new Domain.Entidades.Conversa();
            conversa.DefinirDados(clienteId, prestadorId, DateTime.UtcNow);
            await _conversaRepository.AdicionarAsync(conversa, cancellationToken);
            await _conversaRepository.SalvarAlteracoesAsync(cancellationToken);
        }

        return conversa.ParaDto();
    }
}

public class PortfolioService : IPortfolioService
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IArquivoService _arquivoService;

    public PortfolioService(IPortfolioRepository portfolioRepository, IArquivoService arquivoService)
    {
        _portfolioRepository = portfolioRepository;
        _arquivoService = arquivoService;
    }

    public async Task<PortfolioDto> AdicionarAsync(Guid prestadorId, Stream imagem, string nomeArquivo, string? titulo, string? descricao, CancellationToken cancellationToken = default)
    {
        var urlImagem = await _arquivoService.SalvarAsync(imagem, nomeArquivo, $"portfolio/{prestadorId}", cancellationToken);
        var portfolio = new Domain.Entidades.Portfolio();
        portfolio.DefinirDados(prestadorId, urlImagem, titulo, descricao, DateTime.UtcNow);

        await _portfolioRepository.AdicionarAsync(portfolio, cancellationToken);
        await _portfolioRepository.SalvarAlteracoesAsync(cancellationToken);
        return portfolio.ParaDto();
    }

    public async Task<IEnumerable<PortfolioDto>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        var itens = await _portfolioRepository.ObterPorPrestadorAsync(prestadorId, cancellationToken);
        return itens.Select(p => p.ParaDto());
    }

    public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var portfolio = await _portfolioRepository.ObterPorIdAsync(id, cancellationToken);
        if (portfolio == null)
            return;

        await _arquivoService.ExcluirAsync(portfolio.UrlImagem, cancellationToken);
        _portfolioRepository.Remover(portfolio);
        await _portfolioRepository.SalvarAlteracoesAsync(cancellationToken);
    }
}

public class CertificacaoService : ICertificacaoService
{
    private readonly ICertificacaoRepository _certificacaoRepository;
    private readonly IArquivoService _arquivoService;

    public CertificacaoService(ICertificacaoRepository certificacaoRepository, IArquivoService arquivoService)
    {
        _certificacaoRepository = certificacaoRepository;
        _arquivoService = arquivoService;
    }

    public async Task<CertificacaoDto> AdicionarAsync(Guid prestadorId, Stream documento, string nomeArquivo, string nome, string? instituicao, DateTime? dataEmissao, DateTime? dataValidade, CancellationToken cancellationToken = default)
    {
        var urlDocumento = await _arquivoService.SalvarAsync(documento, nomeArquivo, $"certificacoes/{prestadorId}", cancellationToken);
        var certificacao = new Domain.Entidades.Certificacao();
        certificacao.DefinirDados(prestadorId, nome, instituicao, dataEmissao, dataValidade, urlDocumento, DateTime.UtcNow);

        await _certificacaoRepository.AdicionarAsync(certificacao, cancellationToken);
        await _certificacaoRepository.SalvarAlteracoesAsync(cancellationToken);
        return certificacao.ParaDto();
    }

    public async Task<IEnumerable<CertificacaoDto>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        var itens = await _certificacaoRepository.ObterPorPrestadorAsync(prestadorId, cancellationToken);
        return itens.Select(c => c.ParaDto());
    }

    public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var certificacao = await _certificacaoRepository.ObterPorIdAsync(id, cancellationToken);
        if (certificacao == null)
            return;

        if (!string.IsNullOrEmpty(certificacao.UrlDocumento))
            await _arquivoService.ExcluirAsync(certificacao.UrlDocumento, cancellationToken);

        _certificacaoRepository.Remover(certificacao);
        await _certificacaoRepository.SalvarAlteracoesAsync(cancellationToken);
    }
}
