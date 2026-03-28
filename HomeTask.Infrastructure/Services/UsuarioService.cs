using Microsoft.EntityFrameworkCore;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Entities;
using HomeTask.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;
using HomeTask.Domain.Enums;

namespace HomeTask.Infrastructure.Services;
public class UsuarioService : IUsuarioService
{
    private readonly HomeTaskDbContext _context;

    public UsuarioService(HomeTaskDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .Include(u => u.Cliente)
            .Include(u => u.Prestador)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .Include(u => u.Cliente)
            .Include(u => u.Prestador)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<Usuario> CriarAsync(Usuario usuario, string senha, CancellationToken cancellationToken = default)
    {
        usuario.SenhaHash = HashSenha(senha);
        usuario.DataCadastro = DateTime.UtcNow;

        if (usuario.TipoUsuario == TipoUsuario.Cliente || usuario.TipoUsuario == TipoUsuario.Ambos)
        {
            usuario.Cliente ??= new Cliente 
            { 
                Documento = usuario.Documento,
                TipoUsuario = usuario.TipoUsuario,
                Endereco = usuario.Endereco,
                Cidade = usuario.Cidade,
                Estado = usuario.Estado,
                Cep = usuario.Cep,
                Bairro = usuario.Bairro,
            };
        }

        if (usuario.TipoUsuario == TipoUsuario.Prestador || usuario.TipoUsuario == TipoUsuario.Ambos)
        {
            usuario.Prestador ??= new Prestador 
            { 
                Documento = usuario.Documento,
                TipoUsuario = usuario.TipoUsuario,
                Endereco = usuario.Endereco,
                Cidade = usuario.Cidade,
                Estado = usuario.Estado,
                Cep = usuario.Cep,
                Bairro = usuario.Bairro,
                RaioAtendimentoKm = usuario?.RaioAtendimentoKm,
                Status = usuario?.Status ?? StatusPrestador.EmAnalise,
                MediaAvaliacoes = usuario?.MediaAvaliacoes ?? 0,
                TotalAvaliacoes = usuario?.TotalAvaliacoes ?? 0,
                TotalServicosConcluidos = usuario?.TotalServicosConcluidos ?? 0,
                DataVerificacao = usuario?.DataVerificacao ?? null

            };
        }

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(cancellationToken);

        return usuario;
    }

    public async Task<Usuario> AtualizarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync(cancellationToken);
        return usuario;
    }

    public async Task<bool> ValidarSenhaAsync(string email, string senha, CancellationToken cancellationToken = default)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (usuario == null) return false;

        var senhaHash = HashSenha(senha);
        return usuario.SenhaHash == senhaHash;
    }

    public async Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExisteCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios.AnyAsync(u => u.Documento == cpf, cancellationToken);
    }

    private static string HashSenha(string senha)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
        return Convert.ToBase64String(bytes);
    }
}
