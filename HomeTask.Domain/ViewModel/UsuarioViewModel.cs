using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.ViewModel
{
    public class UsuarioViewModel
    {
            public Guid Id { get; set; } = Guid.NewGuid();

            [MaxLength(100)]
            public string Nome { get; set; }

            [JsonPropertyName("tipo")]
            public TipoUsuario TipoUsuario { get; set; }

            [MaxLength(100)]
            public string Email { get; set; } 

            [MaxLength(256)]
            public string Senha { get; set; }

            [RegularExpression(@"^(\d{3}\.?\d{3}\.?\d{3}-?\d{2}|\d{2}\.?\d{3}\.?\d{3}/?\d{4}-?\d{2})$", ErrorMessage = "Documento inválido. Informe um CPF ou CNPJ válido.")]
            public string Documento { get; set; }
            public string Logradouro { get; set; } = null!;
            public string? Numero { get; set; }
            public string? Complemento { get; set; }
            public string Bairro { get; set; } = null!;
            public string Cep { get; set; } = null!;
            public Guid CidadeId { get; set; }
            [MaxLength(15)]
            public string? Telefone { get; set; }
            public DateTime DataCadastro { get; set; }
            public DateTime? UltimoAcesso { get; set; }
            public bool Ativo { get; set; } = true;
    }
}
