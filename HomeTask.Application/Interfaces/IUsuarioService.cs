using HomeTask.Domain.Contratos;
using HomeTask.Domain.Entidades;

namespace HomeTask.Application.Interfaces;

/// <summary>
/// Interface para gerenciamento de usuários (RF01)
/// </summary>
public interface IUsuarioService
{
    Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Usuario> CriarAsync(Usuario usuario, string senha, CancellationToken cancellationToken = default);
    Task<Usuario> AtualizarAsync(Usuario usuario, CancellationToken cancellationToken = default);
    Task<bool> ValidarSenhaAsync(string email, string senha, CancellationToken cancellationToken = default);
    Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExisteCpfAsync(string cpf, CancellationToken cancellationToken = default);
    Task<bool> AtualizarPerfilAsync(Guid usuarioId, PerfilContrato contrato, CancellationToken cancellationToken = default);

}
