using Application.Features.Video.Shared;
using Application.Interfaces;
using Application.Services.HashTag;
using Domain;
using Domain.Entities.Video;
using MediatR;
using Newtonsoft.Json;

namespace Application.Features.Video.Upload.Dev;

internal class UploadVideoCommandDevHandler(
    IAppDbContext appDbContext,
    IDescriptionParser parser,
    IHashTagService hashtagsService)
    : IRequestHandler<UploadVideoCommandDev, Unit>
{
    public async Task<Unit> Handle(UploadVideoCommandDev request, CancellationToken cancellationToken)
    {
        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(uploadFolder);
        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", request.Key);
            var response = await httpClient.GetStringAsync(request.Url, cancellationToken);

            var json = JsonConvert.DeserializeObject<dynamic>(response);

            foreach (var video in json!.videos)
            {
                string videoUrl = null!;
                foreach (var file in video.video_files)
                    if (file.quality == "hd")
                    {
                        videoUrl = file.link;
                        break;
                    }

                if (videoUrl == null) continue;

                var fileName = $"{Guid.NewGuid()}.mp4";
                var savePath = Path.Combine(uploadFolder, fileName);

                var bytes = await httpClient.GetByteArrayAsync(videoUrl, cancellationToken);
                await File.WriteAllBytesAsync(savePath, bytes, cancellationToken);
                ;

                var randomUserId = request.RandomUserIds[Random.Shared.Next(request.RandomUserIds.Count())];

                var parsedDescription = parser.ParseDescription(request.VideoDescription);
                var newVideo = new VideoEntity
                    { Description = parsedDescription.CleanText, UserId = randomUserId };

                var hashtags = await hashtagsService.GetOrCreateAsync(parsedDescription.Tags);
                foreach (var tag in hashtags)
                    newVideo.HashTags.Add(new VideoHashTagEntity { HashTagId = tag.Id, VideoId = newVideo.Id });

                newVideo.Status = VideoStatus.Processed;
                await appDbContext.Videos.AddAsync(newVideo, cancellationToken);
            }
        }

        await appDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}