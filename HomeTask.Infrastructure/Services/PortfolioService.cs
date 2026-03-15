using HomeTask.Application.Interfaces;
using HomeTask.Domain.Entities;
using HomeTask.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HomeTask.Infrastructure.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly HomeTaskDbContext _context;
        private readonly IArquivoService _arquivoService;

        public PortfolioService(HomeTaskDbContext context, IArquivoService arquivoService)
        {
            _context = context;
            _arquivoService = arquivoService;
        }

        public async Task<Portfolio> AdicionarAsync(Guid prestadorId, Stream imagem, string nomeArquivo, string? titulo, string? descricao, CancellationToken cancellationToken = default)
        {
            var urlImagem = await _arquivoService.SalvarAsync(imagem, nomeArquivo, $"portfolio/{prestadorId}", cancellationToken);

            var portfolio = new Portfolio
            {
                PrestadorId = prestadorId,
                UrlImagem = urlImagem,
                Titulo = titulo,
                Descricao = descricao,
                DataCadastro = DateTime.UtcNow
            };

            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync(cancellationToken);

            return portfolio;
        }

        public async Task<IEnumerable<Portfolio>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
        {
            return await _context.Portfolios
                .AsNoTracking()
                .Where(p => p.PrestadorId == prestadorId)
                .OrderBy(p => p.Ordem)
                .ToListAsync(cancellationToken);
        }

        public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var portfolio = await _context.Portfolios.FindAsync(new object[] { id }, cancellationToken);
            if (portfolio == null) return;

            await _arquivoService.ExcluirAsync(portfolio.UrlImagem, cancellationToken);

            _context.Portfolios.Remove(portfolio);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
