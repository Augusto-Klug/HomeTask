using HomeTask.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HomeTask.Domain.ViewModel
{
    public class UsuarioViewModel
    {
            public Guid Id { get; set; } = Guid.NewGuid();

            [MaxLength(100)]
            public string Nome { get; set; }

            [MaxLength(100)]
            public string Email { get; set; } 

            [MaxLength(256)]
            public string? Senha { get; set; }

        [JsonPropertyName("cpf")]
        [RegularExpression(@"^(\d{3}\.?\d{3}\.?\d{3}-?\d{2}|\d{2}\.?\d{3}\.?\d{3}/?\d{4}-?\d{2})$", ErrorMessage = "Documento inválido. Informe um CPF ou CNPJ válido.")]
            public string Documento { get; set; }

            [MaxLength(15)]
            public string? Telefone { get; set; }

            public TipoUsuario Tipo { get; set; }

            public DateTime DataCadastro { get; set; }

            public DateTime? UltimoAcesso { get; set; }

            public bool Ativo { get; set; } = true;
    }
}
