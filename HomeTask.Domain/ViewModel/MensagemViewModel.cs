using System;
using System.ComponentModel.DataAnnotations;

namespace HomeTask.Domain.ViewModel
{
    public class MensagemViewModel
    {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid RemetenteId { get; set; }
            public Guid? AgendamentoId { get; set; }

            public Guid ConversaId { get; set; }

            [MaxLength(2000)]
            public string Conteudo { get; set; } = string.Empty;

            public DateTime DataEnvio { get; set; }

            public DateTime? DataLeitura { get; set; }

            public bool Lida { get; set; } = false;
    }
}
