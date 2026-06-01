namespace Domain.Entities.Interfaces;

public interface IBannable
{
    public Guid? BannedBy { get; set; }
    public DateTime? BannedAt { get; set; }
    public bool IsBanned => BannedBy.HasValue;
    public void Ban(Guid by);
    public void Unban();
}