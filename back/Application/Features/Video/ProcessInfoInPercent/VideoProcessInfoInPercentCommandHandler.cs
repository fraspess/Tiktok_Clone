using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Video.ProcessInfoInPercent
{
    internal class VideoProcessInfoInPercentCommandHandler(
        IUnitOfWork _uow,
        IVideoProcessingNotifier videoProcessingNotifier) : IRequestHandler<VideoProcessInfoInPercentCommand, Unit>
    {
        public async Task<Unit> Handle(VideoProcessInfoInPercentCommand request, CancellationToken cancellationToken)
        {
            var video = await _uow.Videos.GetByIdAsyncIgnoreQueryFilters(request.VideoId)
                        ?? throw new NotFoundException("Відео не знайдено");

            video.ProccessedInPercents = request.Percentage;
            await videoProcessingNotifier.SendVideoProcessingProgress(request.VideoId, video.UserId,
                request.Percentage);
            await _uow.Videos.UpdateAsync(video);
            await _uow.SaveChangesAsync();
            return Unit.Value;
        }
    }
}