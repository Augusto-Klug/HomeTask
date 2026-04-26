using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Entidades;

/// <summary>
/// Agendamento de serviço entre cliente e prestador (RF04, RF10)
/// </summary>
public class Agendamento
{
    public Agendamento() { }

    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid ClienteId { get; private set; }

    public Cliente Cliente { get; private set; } = null!;

    public Guid PrestadorId { get; private set; }

    public Prestador Prestador { get; private set; } = null!;

    /// <summary>
    /// Data e hora agendada para o serviço
    /// </summary>
    public DateTime DataHoraAgendada { get; private set; }

    /// <summary>
    /// Duração estimada em minutos
    /// </summary>
    public int DuracaoMinutos { get; private set; }

    /// <summary>
    /// Status atual do agendamento
    /// </summary>
    public StatusAgendamento Status { get; private set; } = StatusAgendamento.Solicitado;

    /// <summary>
    /// Endereço de realização do serviço
    /// </summary>
    public Guid EnderecoId { get; private set; }
    public Endereco Endereco { get; private set; } = null!;

    public string? Observacoes { get; private set; }

    /// <summary>
    /// Valor total do serviço
    /// </summary>
    public decimal ValorTotal { get; private set; }

    public DateTime DataSolicitacao { get; private set; } = DateTime.UtcNow;

    public DateTime? DataResposta { get; private set; }

    public DateTime? DataConclusao { get; private set; }

    public string? MotivoRecusa { get; private set; }

    // Navegação
    public Pagamento? Pagamento { get; private set; }
    public Avaliacao? Avaliacao { get; private set; }
    public ICollection<AgendamentoServico> AgendamentoServicos { get; private set; } = [];

    public void AdicionarServico(AgendamentoServico servico)
    {
        AgendamentoServicos.Add(servico);
    }

    public void DefinirDados(
        Guid id,
        Guid clienteId,
        Guid prestadorId,
        DateTime dataHoraAgendada,
        int duracaoMinutos,
        StatusAgendamento status,
        Guid enderecoId,
        string? observacoes,
        decimal valorTotal,
        DateTime dataSolicitacao,
        DateTime? dataResposta,
        DateTime? dataConclusao,
        string? motivoRecusa)
    {
        Id = id;
        ClienteId = clienteId;
        PrestadorId = prestadorId;
        DataHoraAgendada = dataHoraAgendada;
        DuracaoMinutos = duracaoMinutos;
        Status = status;
        EnderecoId = enderecoId;
        Observacoes = observacoes;
        ValorTotal = valorTotal;
        DataSolicitacao = dataSolicitacao;
        DataResposta = dataResposta;
        DataConclusao = dataConclusao;
        MotivoRecusa = motivoRecusa;
    }

    public void DefinirComoSolicitado(DateTime dataSolicitacao)
    {
        Status = StatusAgendamento.Solicitado;
        DataSolicitacao = dataSolicitacao;
    }

    public void Aceitar(DateTime dataResposta)
    {
        Status = StatusAgendamento.Aceito;
        DataResposta = dataResposta;
    }

    public void Recusar(string motivo, DateTime dataResposta)
    {
        Status = StatusAgendamento.Recusado;
        DataResposta = dataResposta;
        MotivoRecusa = motivo;
    }

    public void Iniciar()
    {
        Status = StatusAgendamento.EmAndamento;
    }

    public void Concluir(DateTime dataConclusao)
    {
        Status = StatusAgendamento.Concluido;
        DataConclusao = dataConclusao;
    }

    public void Cancelar(string motivo)
    {
        Status = StatusAgendamento.Cancelado;
        MotivoRecusa = motivo;
    }
}
