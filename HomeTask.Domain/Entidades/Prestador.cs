using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Entidades;

/// <summary>
/// Entidade para prestadores de serviços domésticos (RF02)
/// </summary>
public class Prestador
{
    public Prestador() { }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UsuarioId { get; private set; }
    public Usuario Usuario { get; private set; } = null!;
    public string? Descricao { get; private set; }
    public int? RaioAtendimentoKm { get; private set; } = 10;
    public StatusPrestador Status { get; private set; } = StatusPrestador.EmAnalise;
    public decimal MediaAvaliacoes { get; private set; } = 0;
    public int TotalAvaliacoes { get; private set; } = 0;
    public int TotalServicosConcluidos { get; private set; } = 0;
    public DateTime? DataVerificacao { get; private set; }
    public DateTime? DataPrimeiraNotificacaoBaixaAvaliacao { get; private set; }
    public int? TotalAvaliacoesNaNotificacao { get; private set; }
    public DateTime? DataInicioSuspensao { get; private set; }
    public DateTime? DataFimSuspensao { get; private set; }

    // Navegação
    public ICollection<ServicoPrestador> ServicosOferecidos { get; private set; } = [];
    public ICollection<Agendamento> Agendamentos { get; private set; } = [];
    public ICollection<Avaliacao> Avaliacoes { get; private set; } = [];
    public ICollection<AvaliacaoCliente> AvaliacoesDeClientes { get; private set; } = [];
    public ICollection<Certificacao> Certificacoes { get; private set; } = [];
    public ICollection<Portfolio> Portfolios { get; private set; } = [];
    public ICollection<Disponibilidade> Disponibilidades { get; private set; } = [];
    public ICollection<Conversa> Conversas { get; private set; } = [];

    public void DefinirDados(
        Guid id,
        Guid usuarioId,
        string? descricao,
        int? raioAtendimentoKm,
        StatusPrestador status,
        decimal mediaAvaliacoes,
        int totalAvaliacoes,
        int totalServicosConcluidos,
        DateTime? dataVerificacao,
        DateTime? dataPrimeiraNotificacaoBaixaAvaliacao = null,
        int? totalAvaliacoesNaNotificacao = null,
        DateTime? dataInicioSuspensao = null,
        DateTime? dataFimSuspensao = null)
    {
        Id = id;
        UsuarioId = usuarioId;
        Descricao = descricao;
        RaioAtendimentoKm = raioAtendimentoKm;
        Status = status;
        MediaAvaliacoes = mediaAvaliacoes;
        TotalAvaliacoes = totalAvaliacoes;
        TotalServicosConcluidos = totalServicosConcluidos;
        DataVerificacao = dataVerificacao;
        DataPrimeiraNotificacaoBaixaAvaliacao = dataPrimeiraNotificacaoBaixaAvaliacao;
        TotalAvaliacoesNaNotificacao = totalAvaliacoesNaNotificacao;
        DataInicioSuspensao = dataInicioSuspensao;
        DataFimSuspensao = dataFimSuspensao;
    }

    public void DefinirStatusInicial()
    {
        Status = StatusPrestador.EmAnalise;
        MediaAvaliacoes = 0;
        TotalAvaliacoes = 0;
        TotalServicosConcluidos = 0;
        LimparObservacaoBaixaAvaliacao();
        LimparSuspensaoTemporaria();
    }

    public void DefinirStatus(StatusPrestador status)
    {
        Status = status;
    }

    public void DefinirDataVerificacao(DateTime? dataVerificacao)
    {
        DataVerificacao = dataVerificacao;
    }

    public void AtualizarMetricasAvaliacao(decimal mediaAvaliacoes, int totalAvaliacoes)
    {
        MediaAvaliacoes = mediaAvaliacoes;
        TotalAvaliacoes = totalAvaliacoes;
    }

    public void RegistrarObservacaoBaixaAvaliacao(DateTime dataNotificacao, int totalAvaliacoesNaNotificacao)
    {
        DataPrimeiraNotificacaoBaixaAvaliacao = dataNotificacao;
        TotalAvaliacoesNaNotificacao = totalAvaliacoesNaNotificacao;
    }

    public void LimparObservacaoBaixaAvaliacao()
    {
        DataPrimeiraNotificacaoBaixaAvaliacao = null;
        TotalAvaliacoesNaNotificacao = null;
    }

    public void AplicarSuspensaoTemporaria(DateTime dataInicioSuspensao, DateTime dataFimSuspensao)
    {
        Status = StatusPrestador.Suspenso;
        DataInicioSuspensao = dataInicioSuspensao;
        DataFimSuspensao = dataFimSuspensao;
    }

    public void LimparSuspensaoTemporaria()
    {
        DataInicioSuspensao = null;
        DataFimSuspensao = null;
    }

    public void EncerrarSuspensaoTemporaria()
    {
        Status = StatusPrestador.Ativo;
        LimparSuspensaoTemporaria();
        LimparObservacaoBaixaAvaliacao();
    }

    public void IncrementarTotalServicosConcluidos()
    {
        TotalServicosConcluidos++;
    }
}
