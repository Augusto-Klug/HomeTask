namespace HomeTask.Domain.Entities;

/// <summary>
/// Portfólio do prestador com fotos de trabalhos realizados (RF02)
/// </summary>
public class Portfolio
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PrestadorId { get; set; }

    public Prestador Prestador { get; set; } = null!;

    public string? Titulo { get; set; }

    public string? Descricao { get; set; }

    /// <summary>
    /// URL da imagem
    /// </summary>
    public string UrlImagem { get; set; } = string.Empty;

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public int Ordem { get; set; } = 0;
}
