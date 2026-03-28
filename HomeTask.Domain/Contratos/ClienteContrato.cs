using HomeTask.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HomeTask.Domain.Contratos
{
    public class ClienteContrato
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UsuarioId { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        public string Documento { get; set; }
        public string? Endereco { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Cep { get; set; }
        public string? Bairro { get; set; }

    }
}