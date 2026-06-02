using Application.Interfaces;
using Application.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

internal class StorageService(IConfiguration config, IOptions<AwsS3Options> options) : IStorageService
{
    private readonly string _url = options.Value.Url;
    public string GetVideoThumbnail(Guid videoId)
    {
        return $"{_url}/uploads/{videoId}/thumbnail.jpg";
    }

    public string GetVideoEntryFile(Guid videoId)
    {
        return $"{_url}/uploads/{videoId}/master.m3u8";
    }

    public string GetUserAvatar(Guid userId)
    {
        return $"{_url}/user-images/{userId}"; // /small.webp, /medium.webp, /large.webp
    }

    public Task DeleteUserAvatars(Guid userId)
    {
        throw new NotImplementedException();
    }
}