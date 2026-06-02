using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Persistence;

public class AuditingSaveChanges(ICurrentUser currentUser) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;

        foreach (var entry in dbContext!.ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted || e.State == EntityState.Modified || e.State == EntityState.Added))
        {
            if (entry.State == EntityState.Added && entry.Entity is AuditableEntity entity1)
            {
                entity1.CreatedAt = DateTime.UtcNow;
                entity1.CreatedBy = currentUser.Id;
            }
            
            if (entry.State == EntityState.Modified && entry.Entity is AuditableEntity entity)
            {
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = currentUser.Id;
            }
            
            if (entry.State == EntityState.Deleted && entry.Entity is SoftDeletableEntity deletedEntity)
            {
                entry.State =  EntityState.Modified;
                deletedEntity.DeletedAt = DateTime.UtcNow;
                deletedEntity.DeletedBy = currentUser.Id;
            }

        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}