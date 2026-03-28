using HomeTask.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HomeTask.Domain.ViewModel
{
    public class ClienteViewModel
    {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid UsuarioId { get; set; }
            public TipoUsuario TipoUsuario { get; set; }

            [RegularExpression(@"^(\d{3}\.?\d{3}\.?\d{3}-?\d{2}|\d{2}\.?\d{3}\.?\d{3}/?\d{4}-?\d{2})$", ErrorMessage = "Documento inválido. Informe um CPF ou CNPJ válido.")]
            public string Documento { get; set; }

            [MaxLength(200)]
            public string? Endereco { get; set; }

            [MaxLength(100)]
            public string? Cidade { get; set; }

            [MaxLength(50)]
            public string? Estado { get; set; }

            [MaxLength(10)]
            public string? Cep { get; set; }

            [MaxLength(100)]
            public string? Bairro { get; set; }
    }
}
