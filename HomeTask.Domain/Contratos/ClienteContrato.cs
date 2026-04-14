using HomeTask.Domain.Enums;

namespace HomeTask.Domain.Contratos
{
    public class ClienteContrato
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UsuarioId { get; set; }
    }
}
