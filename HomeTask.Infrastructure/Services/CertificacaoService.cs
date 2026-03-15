using HomeTask.Application.Interfaces;
using HomeTask.Domain.Entities;
using HomeTask.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeTask.Infrastructure.Services;

public class CertificacaoService : ICertificacaoService
{
    private readonly HomeTaskDbContext _context;
    private readonly IArquivoService _arquivoService;

    public CertificacaoService(HomeTaskDbContext context, IArquivoService arquivoService)
    {
        _context = context;
        _arquivoService = arquivoService;
    }

    public async Task<Certificacao> AdicionarAsync(Guid prestadorId, Stream documento, string nomeArquivo, string nome, string? instituicao, DateTime? dataEmissao, DateTime? dataValidade, CancellationToken cancellationToken = default)
    {
        var urlDocumento = await _arquivoService.SalvarAsync(documento, nomeArquivo, $"certificacoes/{prestadorId}", cancellationToken);

        var certificacao = new Certificacao
        {
            PrestadorId = prestadorId,
            Nome = nome,
            Instituicao = instituicao,
            DataEmissao = dataEmissao,
            DataValidade = dataValidade,
            UrlDocumento = urlDocumento,
            DataCadastro = DateTime.UtcNow
        };

        _context.Certificacoes.Add(certificacao);
        await _context.SaveChangesAsync(cancellationToken);

        return certificacao;
    }

    public async Task<IEnumerable<Certificacao>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        return await _context.Certificacoes
            .AsNoTracking()
            .Where(c => c.PrestadorId == prestadorId)
            .OrderByDescending(c => c.DataCadastro)
            .ToListAsync(cancellationToken);
    }

    public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var certificacao = await _context.Certificacoes.FindAsync([id], cancellationToken);
        if (certificacao == null) return;

        if (!string.IsNullOrEmpty(certificacao.UrlDocumento))
            await _arquivoService.ExcluirAsync(certificacao.UrlDocumento, cancellationToken);

        _context.Certificacoes.Remove(certificacao);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
