using Domain.Entities;

namespace Application.Interfaces;

public interface IGenericRepository<TEntity>
    where TEntity : AuditableEntity
{
    Task CreateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task<TEntity?> GetByIdAsync(Guid id);
    IQueryable<TEntity> GetAll();
    public Task<TEntity?> GetByIdAsyncIgnoreQueryFilters(Guid id);
    public TEntity? GetTracked(Func<TEntity, bool> predicate);
    public IQueryable<TEntity> GetAllIgnoreQueryFilters();
}