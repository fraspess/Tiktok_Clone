using Application.Interfaces;
using Domain;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.ProcessFailed;

internal class VideoProcessingFailedCommandHandler(IAppDbContext appDbContext, IVideoProcessingNotifier notifier)
    : IRequestHandler<VideoProcessingFailedCommand, Unit>
{
    public async Task<Unit> Handle(VideoProcessingFailedCommand request, CancellationToken cancellationToken)
    {
        var video = await appDbContext.Videos.IgnoreQueryFilters()
                        .FirstOrDefaultAsync(v => v.Id == request.VideoId, cancellationToken)
                    ?? throw new NotFoundException(ErrorCodes.VideoNotFound);

        video.Status = VideoStatus.Failed;
        video.ProccessedInPercents = 0;

        appDbContext.Videos.Update(video);
        await appDbContext.SaveChangesAsync(cancellationToken);

        await notifier.SendVideoProcessFailed(video.Id, video.UserId, request.Message);
        return Unit.Value;
    }
}