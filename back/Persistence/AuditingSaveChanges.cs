using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Persistence;

public class AuditingSaveChanges(ICurrentUser currentUser) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;

        foreach (var entry in dbContext!.ChangeTracker.Entries().Where(e =>
                     e.State is EntityState.Deleted or EntityState.Modified or EntityState.Added))
        {
            if (entry is { State: EntityState.Added, Entity: AuditableEntity entity1 })
            {
                entity1.CreatedAt = DateTime.UtcNow;
                entity1.CreatedBy = currentUser.Id;
            }

            if (entry is { State: EntityState.Modified, Entity: AuditableEntity entity })
            {
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = currentUser.Id;
            }

            if (entry is not { State: EntityState.Deleted, Entity: SoftDeletableEntity deletedEntity }) continue;
            
            entry.State = EntityState.Modified;
            deletedEntity.IsDeleted = true;
            deletedEntity.DeletedAt = DateTime.UtcNow;
            deletedEntity.DeletedBy = currentUser.Id;
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}