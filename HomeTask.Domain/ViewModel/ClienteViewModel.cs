namespace HomeTask.Domain.ViewModel
{
    public class ClienteViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UsuarioId { get; set; }
    }
}
