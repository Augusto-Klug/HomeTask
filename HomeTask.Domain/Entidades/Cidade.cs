using System;
using System.Collections.Generic;
using System.Text;

namespace HomeTask.Domain.Entidades
{
    public class Cidade
    {
        public Cidade() { }

        public Guid Id { get; private set; } = Guid.NewGuid();

        public String Nome { get; private set; }  = null!;

        public String Estado { get; private set; } = null!;

        public string? CodIBGE { get; private set; }

        public ICollection<Endereco> Enderecos { get; private set; } = [];
    }
}
