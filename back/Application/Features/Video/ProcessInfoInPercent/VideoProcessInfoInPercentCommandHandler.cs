using Application.Interfaces;
using Domain;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.ProcessInfoInPercent;

internal class VideoProcessInfoInPercentCommandHandler(
    IAppDbContext appDbContext,
    IVideoProcessingNotifier videoProcessingNotifier) : IRequestHandler<VideoProcessInfoInPercentCommand, Unit>
{
    public async Task<Unit> Handle(VideoProcessInfoInPercentCommand request, CancellationToken cancellationToken)
    {
        var video = await appDbContext.Videos.IgnoreQueryFilters()
                        .FirstOrDefaultAsync(v => v.Id == request.VideoId, cancellationToken)
                    ?? throw new NotFoundException("Відео не знайдено");

        video.ProccessedInPercents = request.Percentage;
        if (video.Status != VideoStatus.Processing) video.Status = VideoStatus.Processing;
        await videoProcessingNotifier.SendVideoProcessingProgress(request.VideoId, video.UserId,
            request.Percentage);
        appDbContext.Videos.Update(video);
        await appDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}