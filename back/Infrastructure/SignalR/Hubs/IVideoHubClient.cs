namespace Infrastructure.SignalR.Hubs;

public interface IVideoHubClient
{
    Task SendVideoProcessingSucceeded(Guid videoId);

    Task SendVideoProcessingProgress(Guid videoId, int progress);

    Task SendVideoProcessingFailed(Guid videoId, string message);
}