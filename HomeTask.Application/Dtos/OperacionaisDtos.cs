using System.ComponentModel.DataAnnotations;
using HomeTask.Domain.Enums;

namespace HomeTask.Application.Dtos;

public class AvaliacaoDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AgendamentoId { get; set; }
    public Guid ClienteId { get; set; }
    public Guid PrestadorId { get; set; }
    public Guid ServicoPrestadorId { get; set; }

    [Range(0, 5)]
    public int NotaServico { get; set; }

    [Range(0, 5)]
    public int NotaPrestador { get; set; }

    [MaxLength(1000)]
    public string? Comentario { get; set; }

    public DateTime DataAvaliacao { get; set; }
    public bool Visivel { get; set; } = true;
    public string? ClienteNome { get; set; }
    public string? PrestadorNome { get; set; }
    public string? ServicoTitulo { get; set; }
}

public class AvaliacaoClienteDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AgendamentoId { get; set; }
    public Guid ClienteId { get; set; }
    public Guid PrestadorId { get; set; }

    [Range(0, 5)]
    public int Nota { get; set; }

    [MaxLength(1000)]
    public string? Comentario { get; set; }

    public DateTime DataAvaliacao { get; set; }
    public bool Visivel { get; set; } = true;
    public string? ClienteNome { get; set; }
    public string? PrestadorNome { get; set; }
}

public class PagamentoDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AgendamentoId { get; set; }
    public decimal Valor { get; set; }
    public TipoPagamento TipoPagamento { get; set; }
    public StatusPagamento Status { get; set; }

    [MaxLength(100)]
    public string? TransacaoId { get; set; }

    [MaxLength(100)]
    public string? CheckoutExternoId { get; set; }

    [MaxLength(1000)]
    public string? CheckoutUrl { get; set; }

    [MaxLength(100)]
    public string? StatusExterno { get; set; }

    public string? PayloadExterno { get; set; }

    public DateTime DataCriacao { get; set; }
    public DateTime? DataProcessamento { get; set; }
    public DateTime? DataConfirmacao { get; set; }

    [MaxLength(500)]
    public string? MotivoRecusa { get; set; }
}

public class IniciarPagamentoDto
{
    public Guid AgendamentoId { get; set; }
}

public class ReconciliarPagamentoDto
{
    public string PagamentoExternoId { get; set; } = string.Empty;
}

public class PagamentoCheckoutRequestDto
{
    public Guid PagamentoId { get; set; }
    public Guid AgendamentoId { get; set; }
    public decimal Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string ClienteEmail { get; set; } = string.Empty;
    public string? ClienteNome { get; set; }
    public string? ClienteDocumento { get; set; }
}

public class PagamentoCheckoutResponseDto
{
    public string CheckoutExternoId { get; set; } = string.Empty;
    public string CheckoutUrl { get; set; } = string.Empty;
    public string? StatusExterno { get; set; }
    public string? PayloadExterno { get; set; }
}

public class PagamentoStatusGatewayDto
{
    public string PagamentoExternoId { get; set; } = string.Empty;
    public string ReferenciaInterna { get; set; } = string.Empty;
    public StatusPagamento Status { get; set; }
    public string? StatusExterno { get; set; }
    public string? MotivoRecusa { get; set; }
    public string? PayloadExterno { get; set; }
}

public class PagamentoWebhookDto
{
    public string? Topico { get; set; }
    public string? Acao { get; set; }
    public string? PagamentoExternoId { get; set; }
    public string? PayloadExterno { get; set; }
}

public class MensagemDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RemetenteId { get; set; }
    public Guid? AgendamentoId { get; set; }
    public Guid ConversaId { get; set; }

    [MaxLength(2000)]
    public string Conteudo { get; set; } = string.Empty;

    public DateTime DataEnvio { get; set; }
    public DateTime? DataLeitura { get; set; }
    public bool Lida { get; set; }
}

public class ConversaDto
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public Guid PrestadorId { get; set; }
    public DateTime DataCriacao { get; set; }
    public List<MensagemDto> Mensagens { get; set; } = [];
}

public class PortfolioDto
{
    public Guid Id { get; set; }
    public Guid PrestadorId { get; set; }
    public string? Titulo { get; set; }
    public string? Descricao { get; set; }
    public string UrlImagem { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
    public int Ordem { get; set; }
}

public class CertificacaoDto
{
    public Guid Id { get; set; }
    public Guid PrestadorId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Instituicao { get; set; }
    public DateTime? DataEmissao { get; set; }
    public DateTime? DataValidade { get; set; }
    public string? UrlDocumento { get; set; }
    public bool Verificada { get; set; }
    public DateTime DataCadastro { get; set; }
}
