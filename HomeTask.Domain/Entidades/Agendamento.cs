using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Entities;

/// <summary>
/// Agendamento de serviço entre cliente e prestador (RF04, RF10)
/// </summary>
public class Agendamento
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ClienteId { get; set; }

    public Cliente Cliente { get; set; } = null!;

    public Guid PrestadorId { get; set; }

    public Prestador Prestador { get; set; } = null!;

    /// <summary>
    /// Data e hora agendada para o serviço
    /// </summary>
    public DateTime DataHoraAgendada { get; set; }

    /// <summary>
    /// Duração estimada em minutos
    /// </summary>
    public int DuracaoMinutos { get; set; }

    /// <summary>
    /// Status atual do agendamento
    /// </summary>
    public StatusAgendamento Status { get; set; } = StatusAgendamento.Solicitado;

    /// <summary>
    /// Endereço de realização do serviço
    /// </summary>
    public Guid EnderecoId { get; set; }
    public Endereco Endereco { get; set; } = null!;

    public string? Observacoes { get; set; }

    /// <summary>
    /// Valor total do serviço
    /// </summary>
    public decimal ValorTotal { get; set; }

    public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;

    public DateTime? DataResposta { get; set; }

    public DateTime? DataConclusao { get; set; }

    public string? MotivoRecusa { get; set; }

    // Navegação
    public Pagamento? Pagamento { get; set; }
    public Avaliacao? Avaliacao { get; set; }
    public ICollection<AgendamentoServico> AgendamentoServicos { get; set; } = [];
}
