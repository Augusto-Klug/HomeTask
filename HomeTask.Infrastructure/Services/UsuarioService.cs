using HomeTask.Application.Interfaces;
using HomeTask.Domain.Contratos;
using HomeTask.Domain.Entities;
using HomeTask.Domain.Enums;
using HomeTask.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

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
            .Include(u => u.Enderecos)
                .ThenInclude(e => e.Cidade)
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
        usuario.DefinirSenhaHash(HashSenha(senha));
        usuario.DefinirDataCadastro(DateTime.UtcNow);

        // garante que o endereço seja o principal
        var enderecoPrincipal = usuario.Enderecos.FirstOrDefault();
        if (enderecoPrincipal != null)
            enderecoPrincipal.DefinirPrincipal(true);

        if (usuario.TipoUsuario == TipoUsuario.Cliente || usuario.TipoUsuario == TipoUsuario.Ambos)
        {
            if (usuario.Cliente == null)
            {
                usuario.DefinirCliente(new Cliente());
            }
        }

        if (usuario.TipoUsuario == TipoUsuario.Prestador || usuario.TipoUsuario == TipoUsuario.Ambos)
        {
            if (usuario.Prestador == null)
            {
                var prestador = new Prestador();
                prestador.DefinirStatusInicial();
                usuario.DefinirPrestador(prestador);
            }
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

    public async Task<bool> AtualizarPerfilAsync(Guid usuarioId, PerfilContrato contrato, CancellationToken cancellationToken = default)
    {
        var usuarioBanco = await ObterPorIdAsync(usuarioId, cancellationToken);

        if (usuarioBanco == null) {
            return false;
        }

        usuarioBanco.DefinirDados(
            usuarioBanco.Id,
            contrato.Nome,
            usuarioBanco.Email,
            contrato.Documento,
            contrato.Telefone ?? string.Empty,
            usuarioBanco.TipoUsuario,
            usuarioBanco.DataCadastro,
            DateTime.UtcNow,
            usuarioBanco.Ativo
        );

        var enderecoPrincipal = usuarioBanco.Enderecos.FirstOrDefault(e => e.Principal)
                                ?? usuarioBanco.Enderecos.FirstOrDefault();
        if (enderecoPrincipal != null)
        {
            enderecoPrincipal.DefinirDados(
                enderecoPrincipal.UsuarioId,
                enderecoPrincipal.CidadeId,
                contrato.Logradouro ?? string.Empty,
                enderecoPrincipal.Numero,
                enderecoPrincipal.Complemento,
                contrato.Bairro ?? string.Empty,
                contrato.Cep ?? string.Empty,
                true
            );
        }

        if (usuarioBanco.Prestador != null)
        {
            usuarioBanco.Prestador.DefinirDados(
                usuarioBanco.Prestador.Id,
                usuarioBanco.Id,
                contrato.Descricao,
                contrato.RaioAtendimentoKm,
                contrato.Status ?? usuarioBanco.Prestador.Status,
                contrato.MediaAvaliacoes ?? usuarioBanco.Prestador.MediaAvaliacoes,
                contrato.TotalAvaliacoes ?? usuarioBanco.Prestador.TotalAvaliacoes,
                contrato.TotalServicosConcluidos ?? usuarioBanco.Prestador.TotalServicosConcluidos,
                usuarioBanco.Prestador.DataVerificacao
            );
        }

        _context.Usuarios.Update(usuarioBanco);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }


}
