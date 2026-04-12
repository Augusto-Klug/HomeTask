using HomeTask.Domain.Entidades;
using HomeTask.Domain.Entities;

namespace HomeTask.Application.Interfaces;

public interface IEnderecoService
{
    Task<IEnumerable<Endereco>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    Task<Endereco?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Endereco> CriarAsync(Endereco endereco, CancellationToken cancellationToken = default);

    Task<Endereco> AtualizarAsync(Endereco endereco, CancellationToken cancellationToken = default);

    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Define um endereço como principal, desmarcando os demais do usuário
    /// </summary>
    Task DefinirPrincipalAsync(Guid enderecoId, Guid usuarioId, CancellationToken cancellationToken = default);
}
