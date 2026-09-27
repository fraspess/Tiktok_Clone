using Application.Interfaces;
using Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR;

internal class VideoProcessingNotifier(IHubContext<VideoProcessingHub, IVideoHubClient> hub)
    : IVideoProcessingNotifier
{
    public async Task SendVideoProcessFailed(Guid videoId, Guid userId, string message)
    {
        await hub.Clients.User(userId.ToString()).SendVideoProcessingFailed(videoId, message);
    }

    public async Task SendVideoProcessingProgress(Guid videoId, Guid userId, int progress)
    {
        await hub.Clients.User(userId.ToString()).SendVideoProcessingProgress(videoId, progress);
    }

    public async Task SendVideoProcessSucceded(Guid videoId, Guid userId)
    {
        await hub.Clients.User(userId.ToString()).SendVideoProcessingSucceded(videoId);
    }
}