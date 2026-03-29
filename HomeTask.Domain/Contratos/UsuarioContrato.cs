using System;
using System.ComponentModel.DataAnnotations;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Contratos
{
    public class UsuarioContrato
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; }
        public string Email { get; set; } 
        public string Senha { get; set; }
        public string Documento { get; set; } 
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Cep { get; set; }
        public string Bairro { get; set; }
        public int? RaioAtendimentoKm { get; set; }
        public StatusPrestador? Status { get; set; }
        public decimal? MediaAvaliacoes { get; set; }
        public int? TotalAvaliacoes { get; set; }
        public int? TotalServicosConcluidos { get; set; }
        public DateTime? DataVerificacao { get; set; }
        public string? Telefone { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? UltimoAcesso { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
