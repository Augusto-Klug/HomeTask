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
