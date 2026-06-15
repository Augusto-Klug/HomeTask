using System.ComponentModel.DataAnnotations;
using HomeTask.Domain.Enums;

namespace HomeTask.Application.Dtos;

public class AgendamentoDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClienteId { get; set; }
    public Guid PrestadorId { get; set; }
    public Guid? PrincipalServicoPrestadorId { get; set; }
    public DateTime DataHoraAgendada { get; set; }
    public int DuracaoMinutos { get; set; }
    public Guid EnderecoId { get; set; }
    public StatusAgendamento Status { get; set; }

    [MaxLength(500)]
    public string? Observacoes { get; set; }

    public decimal ValorTotal { get; set; }
    public DateTime DataSolicitacao { get; set; }
    public DateTime? DataResposta { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataConclusao { get; set; }

    [MaxLength(500)]
    public string? MotivoRecusa { get; set; }

    public List<Guid> ServicosOferecidosIds { get; set; } = [];
}

public class AgendamentoResumoDto
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public Guid PrestadorId { get; set; }
    public string PrestadorNome { get; set; } = string.Empty;
    public Guid? PrincipalServicoPrestadorId { get; set; }
    public DateTime DataHoraAgendada { get; set; }
    public int DuracaoMinutos { get; set; }
    public StatusAgendamento Status { get; set; }
    public string? Observacoes { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataSolicitacao { get; set; }
    public DateTime? DataResposta { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataConclusao { get; set; }
    public string? MotivoRecusa { get; set; }
    public TipoUsuario? AguardandoRespostaDe { get; set; }
    public bool PodeAvaliar { get; set; }
    public bool Avaliado { get; set; }
    public EnderecoResumoDto Endereco { get; set; } = new();
    public List<ServicoResumoDto> Servicos { get; set; } = [];
}

public class EnderecoResumoDto
{
    public string Logradouro { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}

public class ServicoResumoDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public decimal PrecoBase { get; set; }
    public FormatoCobranca UnidadeCobranca { get; set; }
}
