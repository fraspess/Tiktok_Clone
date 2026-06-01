using Domain.Entities.Interfaces;

namespace Domain.Entities;

public class BannableSoftDeletableEntity : SoftDeletableEntity, IBannable
{
    public Guid? BannedBy { get; set; }
    public DateTime? BannedAt { get; set; }
    public bool IsBanned { get; set; } = false;
    
    public void Ban(Guid by)
    {
        BannedBy = by;
        BannedAt = DateTime.UtcNow;
        IsBanned = true;
    }

    public void Unban()
    {
        BannedBy = null;
        BannedAt = null;
        IsBanned = false;
    }
}