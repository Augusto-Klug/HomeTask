using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Contratos
{
    public class AgendamentoResumoContrato
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public string ClienteNome { get; set; } = string.Empty;
        public Guid PrestadorId { get; set; }
        public string PrestadorNome { get; set; } = string.Empty;
        public DateTime DataHoraAgendada { get; set; }
        public int DuracaoMinutos { get; set; }
        public StatusAgendamento Status { get; set; }
        public string? Observacoes { get; set; }
        public decimal ValorTotal { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public DateTime? DataResposta { get; set; }
        public DateTime? DataConclusao { get; set; }
        public string? MotivoRecusa { get; set; }
        public EnderecoResumoContrato Endereco { get; set; } = new();
        public List<ServicoResumoContrato> Servicos { get; set; } = [];
    }

    public class EnderecoResumoContrato
    {
        public string Logradouro { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }

    public class ServicoResumoContrato
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public decimal PrecoBase { get; set; }
        public FormatoCobranca UnidadeCobranca { get; set; }
    }
}
