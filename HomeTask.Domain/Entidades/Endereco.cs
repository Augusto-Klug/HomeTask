namespace HomeTask.Domain.Entidades
{
    public class Endereco
    {
        public Endereco() { }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid UsuarioId { get; private set; }

        public Guid CidadeId { get; private set; }

        public string Logradouro { get; private set; } = null!;

        public string? Numero { get; private set; }

        public string? Complemento { get; private set;}

        public string Bairro { get; private set; } = null!;

        public string Cep { get; private set; } = null!;

        public Usuario Usuario { get; private set; } = null!;
        public Cidade Cidade { get; private set; } = null!;
        public ICollection<Agendamento> Agendamentos { get; private set; } = [];

        public void DefinirDados(
            Guid cidadeId,
            string logradouro,
            string? numero,
            string? complemento,
            string bairro,
            string cep)
        {
            CidadeId = cidadeId;
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
            Bairro = bairro;
            Cep = cep;
        }

        public void DefinirUsuarioId(Guid usuarioId)
        {
            UsuarioId = usuarioId;
        }

    }
}
