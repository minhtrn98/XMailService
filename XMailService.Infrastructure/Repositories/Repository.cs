using Microsoft.EntityFrameworkCore;
using XMailService.Application.Interfaces;
using XMailService.Domain.Entities.Commons;
using XMailService.Infrastructure.Persistence;

namespace XMailService.Infrastructure.Repositories;

public abstract class Repository<TEntity>(AppDbContext dbContext, ICurrentUserProvider currentUserProvider)
    : IRepository<TEntity>
    where TEntity : class, IEntity
{
    protected ICurrentUserProvider CurrentUserProvider { get; } = currentUserProvider;
    protected DbSet<TEntity> DbSet { get; } = dbContext.Set<TEntity>();
    protected AppDbContext DbContext { get; } = dbContext;

    public IQueryable<TEntity> Query => DbSet;

    public virtual void Add(TEntity entity) => DbSet.Add(entity);

    public virtual void AddRange(IEnumerable<TEntity> entities) => DbSet.AddRange(entities);

    public virtual void Delete(TEntity entity) => DbSet.Remove(entity);

    public virtual void DeleteRange(params TEntity[] entities) => DbSet.RemoveRange(entities);

    public virtual void DeleteRange(IEnumerable<TEntity> entities) => DbSet.RemoveRange(entities);

    public virtual void Update(TEntity entity) => DbSet.Update(entity);

    public virtual void UpdateRange(params TEntity[] entities) => DbSet.UpdateRange(entities);

    public virtual Task<TEntity> First(Ulid id, CancellationToken cancellationToken = default)
        => DbSet.FirstAsync(x => x.Id == id, cancellationToken);

    public virtual Task<TEntity?> FirstOrDefault(Ulid id, CancellationToken cancellationToken = default)
        => DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}
