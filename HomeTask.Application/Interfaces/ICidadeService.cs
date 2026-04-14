using HomeTask.Domain.Entidades;
using HomeTask.Domain.Entities;

namespace HomeTask.Application.Interfaces;

public interface ICidadeService
{
    /// <summary>
    /// Lista todas as cidades ativas para uso em dropdowns
    /// </summary>
    Task<IEnumerable<Cidade>> ListarAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca cidades por nome ou estado para autocomplete
    /// </summary>
    Task<IEnumerable<Cidade>> BuscarAsync(string termo, CancellationToken cancellationToken = default);

    Task<Cidade?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
}
