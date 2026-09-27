namespace Infrastructure.SignalR.Hubs;

public interface IVideoHubClient
{
    Task SendVideoProcessingSucceded(Guid videoId);

    Task SendVideoProcessingProgress(Guid videoId, int progress);

    Task SendVideoProcessingFailed(Guid videoId, string message);
}