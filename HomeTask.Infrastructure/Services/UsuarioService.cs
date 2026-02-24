using Microsoft.EntityFrameworkCore;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Entities;
using HomeTask.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace HomeTask.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de usuários (RF01, RNF10)
/// </summary>
public class UsuarioService : IUsuarioService
{
    private readonly HomeTaskDbContext _context;

    public UsuarioService(HomeTaskDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObterPorIdAsync(Guid id)
    {
        return await _context.Usuarios
            .Include(u => u.Cliente)
            .Include(u => u.Prestador)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        return await _context.Usuarios
            .Include(u => u.Cliente)
            .Include(u => u.Prestador)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario> CriarAsync(Usuario usuario, string senha)
    {
        usuario.SenhaHash = HashSenha(senha);
        usuario.DataCadastro = DateTime.UtcNow;

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return usuario;
    }

    public async Task<Usuario> AtualizarAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<bool> ValidarSenhaAsync(string email, string senha)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        if (usuario == null) return false;

        var senhaHash = HashSenha(senha);
        return usuario.SenhaHash == senhaHash;
    }

    public async Task<bool> ExisteEmailAsync(string email)
    {
        return await _context.Usuarios.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ExisteCpfAsync(string cpf)
    {
        return await _context.Usuarios.AnyAsync(u => u.Cpf == cpf);
    }

    /// <summary>
    /// Hash da senha usando SHA256 (RNF10 - dados sensíveis criptografados)
    /// Em produção, usar BCrypt ou Argon2
    /// </summary>
    private static string HashSenha(string senha)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
        return Convert.ToBase64String(bytes);
    }
}
