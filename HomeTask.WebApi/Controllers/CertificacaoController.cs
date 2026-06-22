using HomeTask.Application.Interfaces;
using HomeTask.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace HomeTask.WebApi.Controllers;

[ApiController]
[Route("api/prestadores/{prestadorId:guid}/certificacoes")]
public class CertificacaoController : ControllerBase
{
    private readonly ICertificacaoService _certificacaoService;

    public CertificacaoController(ICertificacaoService certificacaoService)
    {
        _certificacaoService = certificacaoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CertificacaoDto>>> Listar(Guid prestadorId, CancellationToken cancellationToken)
    {
        var certificacoes = await _certificacaoService.ObterPorPrestadorAsync(prestadorId, cancellationToken);
        return Ok(certificacoes.ToList());
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<CertificacaoDto>> Adicionar(
        Guid prestadorId,
        [FromForm] AdicionarCertificacaoRequest request,
        CancellationToken cancellationToken)
    {
        Stream? stream = null;
        string? nomeArquivo = null;

        if (request.Documento != null)
        {
            stream = request.Documento.OpenReadStream();
            nomeArquivo = request.Documento.FileName;
        }

        var certificacao = await _certificacaoService.AdicionarAsync(
            prestadorId,
            stream!,
            nomeArquivo!,
            request.Nome,
            request.Instituicao,
            request.DataEmissao,
            request.DataValidade,
            cancellationToken);

        return CreatedAtAction(nameof(Listar), new { prestadorId }, certificacao);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid prestadorId, Guid id, CancellationToken cancellationToken)
    {
        await _certificacaoService.RemoverAsync(id, cancellationToken);
        return NoContent();
    }
}
