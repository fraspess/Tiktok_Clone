namespace Domain.Entities.Interfaces;

public interface IBannable
{
    public Guid? BannedBy { get; set; }
    public DateTime? BannedAt { get; set; }
    UserReportReasons? BanReason { get; set; }
    public bool IsBanned => BannedBy.HasValue;
    public void Ban(Guid by, UserReportReasons reason);
    public void Unban();
}