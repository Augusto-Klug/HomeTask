using HomeTask.WebApi.Models.Entities;
using HomeTask.WebApi.Models.Enums;

namespace HomeTask.WebApi.Services.Interfaces;

/// <summary>
/// Interface para gerenciamento de usuários (RF01)
/// </summary>
public interface IUsuarioService
{
    Task<Usuario?> ObterPorIdAsync(int id);
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<Usuario> CriarAsync(Usuario usuario, string senha);
    Task<Usuario> AtualizarAsync(Usuario usuario);
    Task<bool> ValidarSenhaAsync(string email, string senha);
    Task<bool> ExisteEmailAsync(string email);
    Task<bool> ExisteCpfAsync(string cpf);
}
