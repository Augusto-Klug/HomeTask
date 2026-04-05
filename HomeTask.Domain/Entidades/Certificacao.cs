namespace HomeTask.Domain.Entities;

/// <summary>
/// Certificação do prestador de serviço (RF02)
/// </summary>
public class Certificacao
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PrestadorId { get; set; }

    public Prestador Prestador { get; set; } = null!;

    public string Nome { get; set; } = string.Empty;

    public string? Instituicao { get; set; }

    public DateTime? DataEmissao { get; set; }

    public DateTime? DataValidade { get; set; }

    /// <summary>
    /// URL do documento/certificado
    /// </summary>
    public string? UrlDocumento { get; set; }

    public bool Verificada { get; set; } = false;

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}
