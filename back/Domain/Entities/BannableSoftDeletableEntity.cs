using Domain.Entities.Interfaces;

namespace Domain.Entities;

public class BannableSoftDeletableEntity : SoftDeletableEntity, IBannable
{
    public Guid? BannedBy { get; set; }
    public DateTime? BannedAt { get; set; }
    public bool IsBanned { get; set; } = false;
    public UserReportReasons? BanReason { get; set; }
    
    public void Ban(Guid by, UserReportReasons reason)
    {
        BannedBy = by;
        BannedAt = DateTime.UtcNow;
        BanReason = reason;
        IsBanned = true;
    }

    public void Unban()
    {
        BannedBy = null;
        BannedAt = null;
        BanReason = null;
        IsBanned = false;
    }
}