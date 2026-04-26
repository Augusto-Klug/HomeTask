using HomeTask.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeTask.WebApi.Controllers;

[ApiController]
[Route("api/prestadores/{prestadorId:guid}/portfolio")]
public class PortfolioController : ControllerBase
{
    private static readonly string[] _tiposPermitidos = ["image/jpeg", "image/png", "image/webp"];
    private const long _tamanhoMaximo = 5 * 1024 * 1024; // 5 MB

    private readonly IPortfolioService _portfolioService;

    public PortfolioController(IPortfolioService portfolioService)
    {
        _portfolioService = portfolioService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(Guid prestadorId, CancellationToken cancellationToken)
    {
        var itens = await _portfolioService.ObterPorPrestadorAsync(prestadorId, cancellationToken);
        return Ok(itens);
    }

    [HttpPost]
    [RequestSizeLimit(_tamanhoMaximo)]
    public async Task<IActionResult> Adicionar(
        Guid prestadorId,
        IFormFile imagem,
        [FromForm] string? titulo,
        [FromForm] string? descricao,
        CancellationToken cancellationToken)
    {
        if (!_tiposPermitidos.Contains(imagem.ContentType))
            return BadRequest("Tipo de arquivo não permitido. Use JPEG, PNG ou WebP.");

        if (imagem.Length > _tamanhoMaximo)
            return BadRequest("Arquivo excede o tamanho máximo de 5 MB.");

        await using var stream = imagem.OpenReadStream();
        var portfolio = await _portfolioService
            .AdicionarAsync(prestadorId, stream, imagem.FileName, titulo, descricao, cancellationToken);

        return CreatedAtAction(nameof(Listar), new { prestadorId }, portfolio);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid prestadorId, Guid id, CancellationToken cancellationToken)
    {
        await _portfolioService.RemoverAsync(id, cancellationToken);
        return NoContent();
    }
}