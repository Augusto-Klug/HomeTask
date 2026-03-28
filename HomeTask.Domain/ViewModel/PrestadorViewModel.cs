using System;
using System.ComponentModel.DataAnnotations;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.ViewModel
{
    public class PrestadorViewModel
    {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid UsuarioId { get; set; }
            public TipoUsuario TipoUsuario { get; set; }

            [MaxLength(1000)]
            public string? Descricao { get; set; }

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

            public int? RaioAtendimentoKm { get; set; }

            public StatusPrestador Status { get; set; }

            public decimal MediaAvaliacoes { get; set; }

            public int TotalAvaliacoes { get; set; }

            public int TotalServicosConcluidos { get; set; }

            public DateTime? DataVerificacao { get; set; }
    }
}
