using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal class GenericRepository<TEntity>(AppDbContext _context) : IGenericRepository<TEntity>
    where TEntity : AuditableEntity
{
    public async Task CreateAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
    }

    public Task UpdateAsync(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Set<TEntity>()
            .FirstOrDefaultAsync(e => e.Id.Equals(id));
        return entity;
    }

    public IQueryable<TEntity> GetAll()
    {
        return _context.Set<TEntity>();
    }
    

    public TEntity? GetTracked(Func<TEntity, bool> predicate)
    {
        return _context.ChangeTracker
            .Entries<TEntity>()
            .FirstOrDefault(e => predicate(e.Entity))?.Entity;
    }

    public async Task<TEntity?> GetByIdAsyncIgnoreQueryFilters(Guid id)
    {
        return await _context.Set<TEntity>().IgnoreQueryFilters().FirstOrDefaultAsync(v => v.Id.Equals(id));
    }

    public IQueryable<TEntity> GetAllIgnoreQueryFilters()
    {
        return _context.Set<TEntity>().IgnoreQueryFilters();
    }
}