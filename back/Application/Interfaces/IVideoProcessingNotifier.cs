namespace Application.Interfaces;

public interface IVideoProcessingNotifier
{
    public Task SendVideoProcessingProgress(Guid videoId, Guid userId, int progress);

    public Task SendVideoProcessFailed(Guid videoId, Guid userId, string message);

    public Task SendVideoProcessSucceded(Guid videoId, Guid userId);
}