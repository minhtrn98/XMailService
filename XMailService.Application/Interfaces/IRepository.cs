using XMailService.Domain.Entities.Commons;

namespace XMailService.Application.Interfaces;

public interface IRepository<TEntity> where TEntity : IEntity
{
    IQueryable<TEntity> Query { get; }
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void UpdateRange(params TEntity[] entities);
    void Delete(TEntity entity);
    void DeleteRange(params TEntity[] entities);
    void DeleteRange(IEnumerable<TEntity> entities);
    Task<TEntity?> FirstOrDefault(Ulid id, CancellationToken cancellationToken = default);
    Task<TEntity> First(Ulid id, CancellationToken cancellationToken = default);
}
