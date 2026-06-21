using System;
using System.Collections.Generic;
using System.Text;

namespace HomeTask.Domain.Entidades
{
    public class Conversa
    {
        public Conversa() { }
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid ClienteId { get; private set; }
        public Guid PrestadorId { get; private set; }
        public DateTime DataCriacao { get; private set; } = DateTime.UtcNow;
        public Cliente Cliente { get; private set; } = null!;
        public Prestador Prestador { get; private set; } = null!;
        public ICollection<Mensagem> Mensagens { get; private set; } = [];
        public void DefinirDados(Guid clienteId, Guid prestadorId, DateTime dataCriacao)
        {
            ClienteId = clienteId;
            PrestadorId = prestadorId;
            DataCriacao = dataCriacao;
        }
    }
}
