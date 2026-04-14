using System;
using System.Collections.Generic;
using System.Text;

namespace HomeTask.Domain.Entidades
{
    public class Cidade
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public String Nome { get; set; }  = null!;

        public String Estado { get; set; } = null!;

        public string? CodIBGE { get; set; }

        public ICollection<Endereco> Enderecos { get; set; } = [];
    }
}
