using HomeTask.Domain.Repositories;
using HomeTask.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeTask.Infrastructure.Repositories;

public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected readonly HomeTaskDbContext Context;
    protected readonly DbSet<T> DbSet;

    public RepositoryBase(HomeTaskDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual Task<T?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        DbSet.FindAsync([id], cancellationToken).AsTask();

    public Task AdicionarAsync(T entidade, CancellationToken cancellationToken = default) =>
        DbSet.AddAsync(entidade, cancellationToken).AsTask();

    public void Atualizar(T entidade) => DbSet.Update(entidade);

    public void Remover(T entidade) => DbSet.Remove(entidade);

    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default) =>
        Context.SaveChangesAsync(cancellationToken);
}
