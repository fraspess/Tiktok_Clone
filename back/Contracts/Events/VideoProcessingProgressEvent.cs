namespace Contracts.Events;

public record VideoProcessingProgressEvent(Guid VideoId, int Progress);