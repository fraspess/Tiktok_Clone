using Application.Interfaces;
using Domain;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.Processed
{
    internal class VideoProcessedCommandHandler(IAppDbContext appDbContext, IVideoProcessingNotifier _notifier)
        : IRequestHandler<VideoProcessedCommand, Unit>
    {
        public async Task<Unit> Handle(VideoProcessedCommand request, CancellationToken cancellationToken)
        {
            var video = await appDbContext.Videos.IgnoreQueryFilters().FirstOrDefaultAsync(v => v.Id == request.VideoId, cancellationToken: cancellationToken)
                        ?? throw new NotFoundException($"Відео не знайдено {request.VideoId}");

            video.Status = VideoStatus.Processed;
            video.ProccessedInPercents = 100;
            appDbContext.Videos.Update(video);
            await appDbContext.SaveChangesAsync(cancellationToken);

            await _notifier.SendVideoProcessSucceded(request.VideoId, video.UserId);
            return Unit.Value;
        }
    }
}