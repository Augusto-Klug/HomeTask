using HomeTask.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HomeTask.Domain.Entidades
{
    public class Categoria
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Nome { get; set; } = null!;   

        public bool Ativo { get; set; }
        
        public ICollection<ServicoOferecido> ServicosOferecidos { get; set; } = [];

    }
}
