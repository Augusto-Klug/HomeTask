using HomeTask.Application.Dtos;

namespace HomeTask.Application.Interfaces;

public interface ICidadeService
{
    /// <summary>
    /// Lista todas as cidades ativas para uso em dropdowns
    /// </summary>
    Task<IEnumerable<CidadeDto>> ListarAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca cidades por nome ou estado para autocomplete
    /// </summary>
    Task<IEnumerable<CidadeDto>> BuscarAsync(string termo, CancellationToken cancellationToken = default);

    Task<CidadeDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
}
