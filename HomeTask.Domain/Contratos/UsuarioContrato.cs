using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Contratos
{
    public class UsuarioContrato
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Logradouro { get; set; } = null!;
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string Bairro { get; set; } = null!;
        public string Cep { get; set; } = null!;
        public Guid CidadeId { get; set; }
        public string Documento { get; set; }
        public string? Telefone { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? UltimoAcesso { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
