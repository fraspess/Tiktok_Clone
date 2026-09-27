namespace Contracts.Events;

public record VideoProcessingFailedEvent(Guid Id, string Error);