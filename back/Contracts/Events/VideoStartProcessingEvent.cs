namespace Contracts.Events;

public class VideoStartProcessingEvent
{
    public Guid VideoId { get; set; }
    public Guid UserId { get; set; }
}