using HomeTask.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HomeTask.Domain.Entidades
{
    public class Categoria
    {
        public Categoria() { }

        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Nome { get; private set; } = null!;   

        public bool Ativo { get; private set; }
        
        public ICollection<ServicoOferecido> ServicosOferecidos { get; private set; } = [];

    }
}
