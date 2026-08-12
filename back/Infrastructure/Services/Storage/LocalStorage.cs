using Application.Dtos.User;
using Application.Interfaces;
using Application.Options;
using Domain.Exceptions;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Storage;

internal class LocalFileStorageService(IOptions<LocalStorageOptions> options) : IStorageService
{
    private readonly LocalStorageOptions _options = options.Value;

    public string GetVideoThumbnail(Guid videoId)
    {
        return $"{_options.BaseUrl}/uploads/processed/{videoId}/thumbnail.jpg";
    }

    public string GetVideoEntryFile(Guid videoId)
    {
        return $"{_options.BaseUrl}/uploads/processed/{videoId}/master.m3u8";
    }

    public AvatarDto GetUserAvatar(Guid userId)
    {
        var v = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return new AvatarDto
        {
            Small = $"{_options.BaseUrl}/avatars/{userId}/small.webp?v={v}",
            Medium = $"{_options.BaseUrl}/avatars/{userId}/medium.webp?v={v}",
            Large = $"{_options.BaseUrl}/avatars/{userId}/large.webp?v={v}"
        };
    }

    public Task DeleteUserAvatars(Guid userId)
    {
        var dir = Path.Combine(_options.RootPath, "avatars", userId.ToString());
        foreach (var file in new[] { "small.webp", "medium.webp", "large.webp" })
        {
            var path = Path.Combine(dir, file);
            if (File.Exists(path)) File.Delete(path);
        }
        return Task.CompletedTask;
    }

    public Task<string> GetVideoUploadPresignedUrlAsync(Guid videoId, string contentType)
    {
        var allowed = new[] { "video/mp4", "video/quicktime", "video/x-msvideo", "video/webm" };
        if (!allowed.Contains(contentType))
            throw new BadRequestException("Тільки відео файли дозволені");
        
        var url = $"{_options.BaseUrl}/api/videos/{videoId}?contentType={Uri.EscapeDataString(contentType)}";
        return Task.FromResult(url);
    }
}