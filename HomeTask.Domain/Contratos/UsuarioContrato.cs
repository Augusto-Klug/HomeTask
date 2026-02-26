using System;
using System.ComponentModel.DataAnnotations;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Contratos
{
    public class UsuarioContrato
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? Senha { get; set; }

        [MaxLength(14)]
        public string Cpf { get; set; } = string.Empty;

        [MaxLength(15)]
        public string? Telefone { get; set; }

        public TipoUsuario Tipo { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? UltimoAcesso { get; set; }

        public bool Ativo { get; set; } = true;
    }
}
