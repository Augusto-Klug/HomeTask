using HomeTask.Domain.Entities;

namespace HomeTask.Domain.Entidades
{
    public class Endereco
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UsuarioId { get; set; }

        public Guid CidadeId { get; set; }

        public string Logradouro { get; set; } = null!;

        public string? Numero { get; set; }

        public string? Complemento { get; set;}

        public string Bairro { get; set; } = null!;

        public string Cep { get; set; } = null!;

        public bool Principal { get; set; }

        public Usuario Usuario { get; set; } = null!;
        public Cidade Cidade { get; set; } = null!;
        public ICollection<Agendamento> Agendamentos { get; set; } = [];

    }
}