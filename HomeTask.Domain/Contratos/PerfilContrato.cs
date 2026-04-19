using HomeTask.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HomeTask.Domain.Contratos
{
    public class PerfilContrato
    {
        public string Nome { get; set; } = string.Empty;
        public int Tipo { get; set; }
        public string Email { get; set; } 
        public string Documento { get; set; }
        public string? Telefone { get; set; }
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }  
        public string? Bairro { get; set; }
        public string? Estado { get; set; }
        public string? Cidade { get; set; }

        // se for Prestador
        public string? Descricao { get; set; }  
        public int? RaioAtendimentoKm { get; set; }
        public StatusPrestador? Status { get; set; }
        public decimal? MediaAvaliacoes { get; set; }
        public int? TotalAvaliacoes { get; set; }
        public int? TotalServicosConcluidos { get; set; }


    }
}
