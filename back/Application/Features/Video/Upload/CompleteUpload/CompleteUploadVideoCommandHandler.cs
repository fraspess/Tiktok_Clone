using Application.Extensions;
using Application.Features.Video.Shared;
using Application.Interfaces;
using Application.Services.HashTag;
using Contracts;
using Contracts.Events;
using Domain;
using Domain.Entities.Video;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace Application.Features.Video.Upload.CompleteUpload;

public class CompleteUploadVideoCommandHandler(
    IAppDbContext appDbContext,
    IEventBus<VideoStartProcessingEvent> eventBus,
    IDescriptionParser parser,
    ICurrentUser currentUser,
    IHashTagService hashtag,
    IJwtTokenService jwtTokenService) : IRequestHandler<CompleteUploadVideoCommand, Unit>
{
    public async Task<Unit> Handle(CompleteUploadVideoCommand request, CancellationToken cancellationToken)
    {
        var payload = jwtTokenService.ValidateUpdateToken(request.Token);
        if(payload is null || payload.UserId != currentUser.Id) throw new BadRequestException(ErrorCodes.InvalidToken);

        var video = await appDbContext
            .Videos
            .AnyAsync(x => x.Id == payload.VideoId, cancellationToken);
        if (video) throw new BadRequestException(ErrorCodes.AlreadyExists);
        
        var parsedDescription = parser.ParseDescription(request.Description);
        var newVideo = new VideoEntity
        {
            Id = payload.VideoId,
            ShortId = await Nanoid.GenerateAsync("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 9),
            UserId = currentUser.Id!.Value,
            Description = parsedDescription.CleanText,
            Status = VideoStatus.Pending,
            ProccessedInPercents = 0
        };

        var hashtags = await hashtag.GetOrCreateAsync(parsedDescription.Tags);
        foreach (var tag in hashtags)
            newVideo.HashTags.Add(new VideoHashTagEntity { HashTagId = tag.Id });

        await appDbContext.Videos.AddAsync(newVideo, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);
        await eventBus.PublishAsync(new VideoStartProcessingEvent { VideoId = payload.VideoId});
        return Unit.Value;
    }
}