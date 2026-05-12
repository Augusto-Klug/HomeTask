namespace HomeTask.Domain.Repositories;

public interface IRepositoryBase<T> where T : class
{
    Task<T?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AdicionarAsync(T entidade, CancellationToken cancellationToken = default);
    void Atualizar(T entidade);
    void Remover(T entidade);
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}
