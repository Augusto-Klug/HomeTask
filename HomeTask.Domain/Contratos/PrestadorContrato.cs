using System;
using System.ComponentModel.DataAnnotations;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Contratos
{
    public class PrestadorContrato
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UsuarioId { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        public string? Descricao { get; set; }
        public string Documento { get; set; }
        public string? Endereco { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Cep { get; set; }
        public string? Bairro { get; set; }
        public int? RaioAtendimentoKm { get; set; } = 10;
        public StatusPrestador Status { get; set; }
        public decimal MediaAvaliacoes { get; set; }
        public int TotalAvaliacoes { get; set; }
        public int TotalServicosConcluidos { get; set; }
        public DateTime? DataVerificacao { get; set; }
    }
}
