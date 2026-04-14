using HomeTask.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeTask.Domain.Entidades
{
    public class Conversa
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ClienteId { get; set; }

        public Guid PrestadorId { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public Cliente Cliente { get; set; } = null!;

        public Prestador Prestador { get; set; } = null!;

        public ICollection<Mensagem> Mensagens { get; set; } = [];

    }
}
