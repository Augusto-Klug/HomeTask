using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Entities;

public class Usuario
{
    public Usuario() { }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string SenhaHash { get; private set; }
    public string Documento { get; private set; }
    public string? Telefone { get; private set; }
    public TipoUsuario TipoUsuario { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public DateTime? UltimoAcesso { get; private set; }
    public bool Ativo { get; private set; } = true;

    // Navegação
    public Cliente? Cliente { get; private set; }
    public Prestador? Prestador { get; private set; }
    public ICollection<Endereco> Enderecos { get; private set; } = [];

    public void DefinirDados(
        Guid id,
        string nome,
        string email,
        string documento,
        string? telefone,
        TipoUsuario tipoUsuario,
        DateTime dataCadastro,
        DateTime? ultimoAcesso,
        bool ativo)
    {
        Id = id;
        Nome = nome;
        Email = email;
        Documento = documento;
        Telefone = telefone;
        TipoUsuario = tipoUsuario;
        DataCadastro = dataCadastro;
        UltimoAcesso = ultimoAcesso;
        Ativo = ativo;
    }

    public void DefinirSenhaHash(string senhaHash)
    {
        SenhaHash = senhaHash;
    }

    public void DefinirDataCadastro(DateTime dataCadastro)
    {
        DataCadastro = dataCadastro;
    }

    public void DefinirCliente(Cliente? cliente)
    {
        Cliente = cliente;
    }

    public void DefinirPrestador(Prestador? prestador)
    {
        Prestador = prestador;
    }

    public void AdicionarEnderecos(Endereco endereco)
    {
        Enderecos.Add(endereco);
    }

}
