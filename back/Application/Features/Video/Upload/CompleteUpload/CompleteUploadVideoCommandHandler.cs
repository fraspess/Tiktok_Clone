using Application.Features.Video.Shared;
using Application.Interfaces;
using Application.Services.HashTag;
using Contracts;
using Contracts.Events;
using Domain;
using Domain.Entities.Video;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.Upload.CompleteUpload;

public class CompleteUploadVideoCommandHandler(
    IAppDbContext appDbContext,
    IEventBus<VideoStartProcessingEvent> eventBus,
    IDescriptionParser _parser,
    ICurrentUser currentUser,
    IHashTagService _hashtag) : IRequestHandler<CompleteUploadVideoCommand, Unit>
{
    public async Task<Unit> Handle(CompleteUploadVideoCommand request, CancellationToken cancellationToken)
    {
        var exists = await appDbContext
            .Videos
            .IgnoreQueryFilters()
            .AnyAsync(v => v.Id == request.VideoId, cancellationToken);
        if (exists is true) throw new BadRequestException("Відео уже існує");
        var parsedDescription = _parser.ParseDescription(request.Description);
        var newVideo = new VideoEntity
        {
            Id = request.VideoId,
            UserId = currentUser.Id!.Value,
            Description = parsedDescription.CleanText,
            Status = VideoStatus.Pending,
            ProccessedInPercents = 0
        };

        var hashtags = await _hashtag.GetOrCreateAsync(parsedDescription.Tags);
        foreach (var tag in hashtags)
            newVideo.HashTags.Add(new VideoHashTagEntity { HashTagId = tag.Id });

        await appDbContext.Videos.AddAsync(newVideo, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);
        await eventBus.PublishAsync(new VideoStartProcessingEvent { VideoId = request.VideoId });
        return Unit.Value;
    }
}