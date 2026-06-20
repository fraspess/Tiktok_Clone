using Application.Dtos.User;
using Application.Dtos.Video;
using Application.Interfaces;
using Riok.Mapperly.Abstractions;

namespace Application.Mapper;

[Mapper]
public partial class VideoMapper(IStorageService storageService)
{
    [MapProperty(nameof(VideoProjectionDto.Author.Id), nameof(VideoDto.Author.Id))]
    [MapProperty(nameof(VideoProjectionDto.Author.Username), nameof(VideoDto.Author.Username))]
    [MapProperty(nameof(VideoProjectionDto.Id), nameof(VideoDto.VideoUrl), Use = nameof(GetVideoUrl))]
    [MapProperty(nameof(VideoProjectionDto.Author.Id), nameof(VideoDto.Author.Avatar), Use = nameof(GetAvatarUrl))]
    [MapProperty(nameof(VideoProjectionDto.Id), nameof(VideoDto.ThumbnailUrl), Use = nameof(GetThumbnailUrl))]
    public partial VideoDto ToDto(VideoProjectionDto source);

    [MapProperty(nameof(VideoProjectionDto.Author.Id), nameof(VideoDto.Author.Id))]
    [MapProperty(nameof(VideoProjectionDto.Author.Username), nameof(VideoDto.Author.Username))]
    [MapProperty(nameof(VideoProjectionDto.Id), nameof(VideoDto.VideoUrl), Use = nameof(GetVideoUrl))]
    [MapProperty(nameof(VideoProjectionDto.Author.Id), nameof(VideoDto.Author.Avatar), Use = nameof(GetAvatarUrl))]
    [MapProperty(nameof(VideoProjectionDto.Id), nameof(VideoDto.ThumbnailUrl), Use = nameof(GetThumbnailUrl))]
    public partial SimpleVideoDto ToSimpleDto(VideoProjectionDto source);

    [MapProperty(nameof(VideoProjectionDto.Author.Id), nameof(MyVideoDto.Author.Id))]
    [MapProperty(nameof(VideoProjectionDto.Author.Username), nameof(MyVideoDto.Author.Username))]
    [MapProperty(nameof(VideoProjectionDto.Id), nameof(MyVideoDto.VideoUrl), Use = nameof(GetVideoUrl))]
    [MapProperty(nameof(VideoProjectionDto.Author.Id), nameof(MyVideoDto.Author.Avatar), Use = nameof(GetAvatarUrl))]
    [MapProperty(nameof(VideoProjectionDto.Id), nameof(MyVideoDto.ThumbnailUrl), Use = nameof(GetThumbnailUrl))]
    [MapProperty(nameof(VideoProjectionDto.Status), nameof(MyVideoDto.Status))]
    public partial MyVideoDto ToMyDto(VideoProjectionDto source);


    [UserMapping(Default = false)]
    private string GetVideoUrl(Guid videoId)
    {
        return storageService.GetVideoEntryFile(videoId);
    }

    [UserMapping(Default = false)]
    private object GetAvatarUrl(Guid userId)
    {
        return storageService.GetUserAvatar(userId);
    }

    [UserMapping(Default = false)]
    private string GetThumbnailUrl(Guid videoId)
    {
        return storageService.GetVideoThumbnail(videoId);
    }
}