using System.ComponentModel.DataAnnotations;
using Amazon.S3;
using Amazon.S3.Model;
using Application.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using ValidationException = Domain.Exceptions.ValidationException;

namespace Infrastructure.Services.Storage;

internal class S3StorageService(IAmazonS3 s3Client, IOptions<AwsS3Options> options) : IStorageService
{
    private readonly AwsS3Options _options = options.Value;
    public string GetVideoThumbnail(Guid videoId)
    {
        return $"{_options.CdnScheme}://{_options.VideoCdnDomain}/{videoId}/thumbnail.jpg";
    }

    public string GetVideoEntryFile(Guid videoId)
    {
        return $"{_options.CdnScheme}://{_options.VideoCdnDomain}/{videoId}/master.m3u8";
    }

    public string GetUserAvatar(Guid userId)
    {
        return $"{_options.CdnScheme}://{_options.AvatarsCdnDomain}/{userId}";
    }

    public Task DeleteUserAvatars(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetVideoUploadPresignedUrlAsync(Guid videoId, string contentType)
    {
        var allowed = new[] { "video/mp4", "video/quicktime", "video/x-msvideo", "video/webm" };
        if (!allowed.Contains(contentType))
            throw new ValidationException("Тільки відео файли дозволені");

        var url = s3Client.GetPreSignedURL(new GetPreSignedUrlRequest()
        {
            BucketName = _options.UploadsBucketName,
            Key = $"unprocessed/{videoId}/original",
            Verb = HttpVerb.PUT,
            Expires = DateTime.Now.AddMinutes(15),
            ContentType = contentType
        });

        return Task.FromResult(url);
    }
}