using Application.Dtos.User;

namespace Application.Interfaces;

public interface IStorageService
{
    public string GetVideoThumbnail(Guid videoId);
    public string GetVideoEntryFile(Guid videoId);
    public AvatarDto GetUserAvatar(Guid userId);
    public Task DeleteUserAvatars(Guid userId);
    public Task<string> GetVideoUploadPresignedUrlAsync(Guid videoId, string contentType);
}