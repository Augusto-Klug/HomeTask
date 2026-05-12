using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using HomeTask.Domain.Enums;

namespace HomeTask.Application.Dtos;

public class UsuarioDto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("tipo")]
    public TipoUsuario TipoUsuario { get; set; }

    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(256)]
    public string Senha { get; set; } = string.Empty;

    [RegularExpression(@"^(\d{3}\.?\d{3}\.?\d{3}-?\d{2}|\d{2}\.?\d{3}\.?\d{3}/?\d{4}-?\d{2})$", ErrorMessage = "Documento inválido. Informe um CPF ou CNPJ válido.")]
    public string Documento { get; set; } = string.Empty;

    public string Logradouro { get; set; } = string.Empty;
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public Guid CidadeId { get; set; }

    [MaxLength(15)]
    public string? Telefone { get; set; }

    public DateTime DataCadastro { get; set; }
    public DateTime? UltimoAcesso { get; set; }
    public bool Ativo { get; set; } = true;
}

public class PerfilDto
{
    public string Nome { get; set; } = string.Empty;
    public int Tipo { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Cep { get; set; }
    public string? Logradouro { get; set; }
    public string? Bairro { get; set; }
    public string? Estado { get; set; }
    public string? Cidade { get; set; }
    public string? Descricao { get; set; }
    public int? RaioAtendimentoKm { get; set; }
    public StatusPrestador? Status { get; set; }
    public decimal? MediaAvaliacoes { get; set; }
    public int? TotalAvaliacoes { get; set; }
    public int? TotalServicosConcluidos { get; set; }
}

public class ClienteDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UsuarioId { get; set; }
}

public class PrestadorDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UsuarioId { get; set; }

    [MaxLength(1000)]
    public string? Descricao { get; set; }

    public int? RaioAtendimentoKm { get; set; }
    public StatusPrestador Status { get; set; }
    public decimal MediaAvaliacoes { get; set; }
    public int TotalAvaliacoes { get; set; }
    public int TotalServicosConcluidos { get; set; }
    public DateTime? DataVerificacao { get; set; }
}

public class CidadeDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? CodIBGE { get; set; }
}
