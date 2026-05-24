using HomeTask.Application.Dtos;

namespace HomeTask.Application.Interfaces;

/// <summary>
/// Interface para gerenciamento de usuários (RF01)
/// </summary>
public interface IUsuarioService
{
    Task<UsuarioDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UsuarioDto?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UsuarioDto> CriarAsync(UsuarioDto usuario, string senha, CancellationToken cancellationToken = default);
    Task<UsuarioDto> AtualizarAsync(UsuarioDto usuario, CancellationToken cancellationToken = default);
    Task<bool> ValidarSenhaAsync(string email, string senha, CancellationToken cancellationToken = default);
    Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExisteCpfAsync(string cpf, CancellationToken cancellationToken = default);
    Task<bool> AtualizarPerfilAsync(Guid usuarioId, PerfilDto dto, CancellationToken cancellationToken = default);
    Task<PerfilDto?> ObterPerfilAsync(Guid usuarioId, CancellationToken cancellationToken = default);
}
