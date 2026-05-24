namespace HomeTask.Domain.Entidades;

/// <summary>
/// Portfólio do prestador com fotos de trabalhos realizados (RF02)
/// </summary>
public class Portfolio
{
    public Portfolio() { }

    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid PrestadorId { get; private set; }

    public Prestador Prestador { get; private set; } = null!;

    public string? Titulo { get; private set; }

    public string? Descricao { get; private set; }

    /// <summary>
    /// URL da imagem
    /// </summary>
    public string UrlImagem { get; private set; } = string.Empty;

    public DateTime DataCadastro { get; private set; } = DateTime.UtcNow;

    public int Ordem { get; private set; } = 0;

    public void DefinirDados(
        Guid prestadorId,
        string urlImagem,
        string? titulo,
        string? descricao,
        DateTime dataCadastro)
    {
        PrestadorId = prestadorId;
        UrlImagem = urlImagem;
        Titulo = titulo;
        Descricao = descricao;
        DataCadastro = dataCadastro;
    }
}
