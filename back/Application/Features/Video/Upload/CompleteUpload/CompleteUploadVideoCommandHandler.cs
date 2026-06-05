using Application.Interfaces;
using Contracts;
using Contracts.Events;
using Domain;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.Upload.CompleteUpload;

public class CompleteUploadVideoCommandHandler(IAppDbContext appDbContext, IEventBus<VideoStartProcessingEvent> eventBus) : IRequestHandler<CompleteUploadVideoCommand, Unit>
{
    public async Task<Unit> Handle(CompleteUploadVideoCommand request, CancellationToken cancellationToken)
    {
        var video = await appDbContext.Videos.FirstOrDefaultAsync(v => v.Id == request.VideoId, cancellationToken: cancellationToken)
            ?? throw new NotFoundException("Відео не знайдено");

        video.Status = VideoStatus.Processing;
        appDbContext.Videos.Update(video);
        await eventBus.PublishAsync(new VideoStartProcessingEvent() { VideoId = request.VideoId });
        await appDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}