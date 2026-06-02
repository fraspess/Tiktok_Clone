using Application.Interfaces;
using Domain;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Video.Processed
{
    internal class VideoProcessedCommandHandler(IUnitOfWork _uow, IVideoProcessingNotifier _notifier)
        : IRequestHandler<VideoProcessedCommand, Unit>
    {
        public async Task<Unit> Handle(VideoProcessedCommand request, CancellationToken cancellationToken)
        {
            var video = await _uow.Videos.GetByIdAsyncIgnoreQueryFilters(request.VideoId)
                        ?? throw new NotFoundException($"Відео не знайдено {request.VideoId}");

            video.Status = VideoStatus.Processed;
            video.ProccessedInPercents = 100;
            await _uow.Videos.UpdateAsync(video);
            await _uow.SaveChangesAsync();

            await _notifier.SendVideoProcessSucceded(request.VideoId, request.UserId);
            return Unit.Value;
        }
    }
}