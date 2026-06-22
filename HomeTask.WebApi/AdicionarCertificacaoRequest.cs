using Microsoft.AspNetCore.Http;

namespace HomeTask.WebApi;

public sealed class AdicionarCertificacaoRequest
{
    public string Nome { get; set; } = string.Empty;
    public string? Instituicao { get; set; }
    public DateTime? DataEmissao { get; set; }
    public DateTime? DataValidade { get; set; }
    public IFormFile? Documento { get; set; }
}
