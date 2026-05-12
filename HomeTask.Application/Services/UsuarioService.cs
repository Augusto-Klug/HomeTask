using System.Security.Cryptography;
using System.Text;
using HomeTask.Application.Dtos;
using HomeTask.Application.Interfaces;
using HomeTask.Application.Mappings;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;
using HomeTask.Domain.Repositories;

namespace HomeTask.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<UsuarioDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(id, cancellationToken);
        return usuario?.ParaDto();
    }

    public async Task<UsuarioDto?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(email, cancellationToken);
        return usuario?.ParaDto();
    }

    public async Task<UsuarioDto> CriarAsync(UsuarioDto usuarioDto, string senha, CancellationToken cancellationToken = default)
    {
        var usuario = usuarioDto.ParaEntidade();
        usuario.DefinirSenhaHash(HashSenha(senha));
        usuario.DefinirDataCadastro(DateTime.UtcNow);

        if ((usuario.TipoUsuario == TipoUsuario.Cliente || usuario.TipoUsuario == TipoUsuario.Ambos) && usuario.Cliente == null)
            usuario.DefinirCliente(new Cliente());

        if ((usuario.TipoUsuario == TipoUsuario.Prestador || usuario.TipoUsuario == TipoUsuario.Ambos) && usuario.Prestador == null)
        {
            var prestador = new Prestador();
            prestador.DefinirStatusInicial();
            usuario.DefinirPrestador(prestador);
        }

        await _usuarioRepository.AdicionarAsync(usuario, cancellationToken);
        await _usuarioRepository.SalvarAlteracoesAsync(cancellationToken);
        return usuario.ParaDto();
    }

    public async Task<UsuarioDto> AtualizarAsync(UsuarioDto usuarioDto, CancellationToken cancellationToken = default)
    {
        var usuario = usuarioDto.ParaEntidade();
        _usuarioRepository.Atualizar(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync(cancellationToken);
        return usuario.ParaDto();
    }

    public async Task<bool> ValidarSenhaAsync(string email, string senha, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(email, cancellationToken);
        if (usuario == null)
            return false;

        return usuario.SenhaHash == HashSenha(senha);
    }

    public Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken = default) =>
        _usuarioRepository.ExisteEmailAsync(email, cancellationToken);

    public Task<bool> ExisteCpfAsync(string cpf, CancellationToken cancellationToken = default) =>
        _usuarioRepository.ExisteCpfAsync(cpf, cancellationToken);

    public async Task<bool> AtualizarPerfilAsync(Guid usuarioId, PerfilDto dto, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId, cancellationToken);
        if (usuario == null)
            return false;

        usuario.DefinirDados(
            usuario.Id,
            dto.Nome,
            usuario.Email,
            dto.Documento,
            dto.Telefone ?? string.Empty,
            usuario.TipoUsuario,
            usuario.DataCadastro,
            DateTime.UtcNow,
            usuario.Ativo);

        var endereco = usuario.Endereco;
        if (endereco != null)
        {
            endereco.DefinirDados(
                endereco.CidadeId,
                dto.Logradouro ?? string.Empty,
                endereco.Numero,
                endereco.Complemento,
                dto.Bairro ?? string.Empty,
                dto.Cep ?? string.Empty);
        }

        if (usuario.Prestador != null)
        {
            usuario.Prestador.DefinirDados(
                usuario.Prestador.Id,
                usuario.Id,
                dto.Descricao,
                dto.RaioAtendimentoKm,
                dto.Status ?? usuario.Prestador.Status,
                dto.MediaAvaliacoes ?? usuario.Prestador.MediaAvaliacoes,
                dto.TotalAvaliacoes ?? usuario.Prestador.TotalAvaliacoes,
                dto.TotalServicosConcluidos ?? usuario.Prestador.TotalServicosConcluidos,
                usuario.Prestador.DataVerificacao);
        }

        _usuarioRepository.Atualizar(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync(cancellationToken);
        return true;
    }

    public async Task<PerfilDto?> ObterPerfilAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId, cancellationToken);
        return usuario?.ParaPerfilDto();
    }

    private static string HashSenha(string senha)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
        return Convert.ToBase64String(bytes);
    }
}
