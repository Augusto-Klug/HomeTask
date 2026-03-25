using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Entities;

/// <summary>
/// Serviço oferecido por um prestador (RF02, RF03)
/// </summary>
public class ServicoOferecido
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PrestadorId { get; set; }

    public Prestador Prestador { get; set; } = null!;

    public CategoriaServico Categoria { get; set; }

    public string? Titulo { get; set; }

    public string? Descricao { get; set; }

    /// <summary>
    /// Preço base do serviço
    /// </summary>
    public decimal PrecoBase { get; set; }

    /// <summary>
    /// Unidade de cobrança (hora, diária, serviço)
    /// </summary>
    public string UnidadeCobranca { get; set; } = "hora";

    /// <summary>
    /// Duração estimada em minutos
    /// </summary>
    public int DuracaoEstimadaMinutos { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}
