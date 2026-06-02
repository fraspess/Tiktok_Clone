namespace Domain.Entities.Interfaces;

public interface ISoftDeletable
{
    public Guid? DeletedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public void Delete(Guid by);
}