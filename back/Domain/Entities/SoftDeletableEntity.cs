using Domain.Entities.Interfaces;

namespace Domain.Entities;

public abstract class SoftDeletableEntity : AuditableEntity, ISoftDeletable
{
    public Guid? DeletedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public void Delete(Guid by)
    {
        DeletedBy = by;
        DeletedAt = DateTime.UtcNow;
    }
}