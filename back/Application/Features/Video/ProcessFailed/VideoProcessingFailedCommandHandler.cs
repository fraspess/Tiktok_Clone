using Application.Interfaces;
using Domain;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Video.ProcessFailed
{
    internal class VideoProcessingFailedCommandHandler(IUnitOfWork _uow, IVideoProcessingNotifier notifier)
        : IRequestHandler<VideoProcessingFailedCommand, Unit>
    {
        public async Task<Unit> Handle(VideoProcessingFailedCommand request, CancellationToken cancellationToken)
        {
            var video = await _uow.Videos.GetByIdAsyncIgnoreQueryFilters(request.VideoId)
                        ?? throw new NotFoundException("Відео не знайдено");

            video.Status = VideoStatus.Failed;
            video.ProccessedInPercents = 0;

            await _uow.Videos.UpdateAsync(video);
            await _uow.SaveChangesAsync();

            await notifier.SendVideoProcessFailed(video.Id, video.UserId, request.Message);
            return Unit.Value;
        }
    }
}