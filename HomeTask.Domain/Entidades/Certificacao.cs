namespace HomeTask.Domain.Entidades;

/// <summary>
/// Certificação do prestador de serviço (RF02)
/// </summary>
public class Certificacao
{
    public Certificacao() { }

    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid PrestadorId { get; private set; }

    public Prestador Prestador { get; private set; } = null!;

    public string Nome { get; private set; } = string.Empty;

    public string? Instituicao { get; private set; }

    public DateTime? DataEmissao { get; private set; }

    public DateTime? DataValidade { get; private set; }

    /// <summary>
    /// URL do documento/certificado
    /// </summary>
    public string? UrlDocumento { get; private set; }

    public bool Verificada { get; private set; } = false;

    public DateTime DataCadastro { get; private set; } = DateTime.UtcNow;

    public void DefinirDados(
        Guid prestadorId,
        string nome,
        string? instituicao,
        DateTime? dataEmissao,
        DateTime? dataValidade,
        string? urlDocumento,
        DateTime dataCadastro)
    {
        PrestadorId = prestadorId;
        Nome = nome;
        Instituicao = instituicao;
        DataEmissao = dataEmissao;
        DataValidade = dataValidade;
        UrlDocumento = urlDocumento;
        DataCadastro = dataCadastro;
    }
}
