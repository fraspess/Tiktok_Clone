using Amazon.S3;
using Application.Features.Video.Shared;
using Application.Interfaces;
using Application.Services.HashTag;
using Contracts;
using Contracts.Events;
using Domain;
using Domain.Entities.Video;
using MediatR;

namespace Application.Features.Video.Upload
{
    internal class UploadVideoCommandHandler(
        IAppDbContext appDbContext,
        IDescriptionParser _parser,
        IHashTagService _hashtag,
        IEventBus<VideoStartProcessingEvent> eventBus,
        ICurrentUser currentUser,
        IStorageService storageService) : IRequestHandler<UploadVideoCommand, string>
    {
        public async Task<string> Handle(UploadVideoCommand request, CancellationToken cancellationToken)
        {
            var parsedDescription = _parser.ParseDescription(request.Dto.Description);
            var newVideo = new VideoEntity()
            {
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

            /*var tempFilePath = await tempVideoStorage.SaveVideoAsync(request.Dto.VideoFile);
            await eventBus.PublishAsync(new VideoStartProcessingEvent
                { FilePath = tempFilePath, VideoId = newVideo.Id, UserId = currentUser.Id.Value});*/
            return await storageService.GetVideoUploadPresignedUrlAsync(newVideo.Id, request.Dto.ContentType);
        }
    }
}