using System.ComponentModel.DataAnnotations;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
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
        return $"{_options.CdnScheme}://{_options.CdnDomain}/uploads/processed/{videoId}/thumbnail.jpg";
    }

    public string GetVideoEntryFile(Guid videoId)
    {
        return $"{_options.CdnScheme}://{_options.CdnDomain}/uploads/processed/{videoId}/master.m3u8";
    }

    public object GetUserAvatar(Guid userId)
    {
        return new {
            Small = $"{_options.CdnScheme}://{_options.CdnDomain}/avatars/{userId}/small.webp",
            Medium = $"{_options.CdnScheme}://{_options.CdnDomain}/avatars/{userId}/medium.webp",
            Large = $"{_options.CdnScheme}://{_options.CdnDomain}/avatars/{userId}/large.webp"
        };
    }

    public async Task DeleteUserAvatars(Guid userId)
    {
        await s3Client.DeleteObjectAsync(_options.BucketName, $"avatars/{userId}/small.webp");
        await s3Client.DeleteObjectAsync(_options.BucketName, $"avatars/{userId}/medium.webp");
        await s3Client.DeleteObjectAsync(_options.BucketName, $"avatars/{userId}/large.webp");
    }

    public Task<string> GetVideoUploadPresignedUrlAsync(Guid videoId, string contentType)
    {
        var allowed = new[] { "video/mp4", "video/quicktime", "video/x-msvideo", "video/webm" };
        if (!allowed.Contains(contentType))
            throw new ValidationException("Тільки відео файли дозволені");

        var url = s3Client.GetPreSignedURL(new GetPreSignedUrlRequest()
        {
            BucketName = _options.BucketName,
            Key = $"uploads/unprocessed/{videoId}/original",
            Verb = HttpVerb.PUT,
            Expires = DateTime.Now.AddMinutes(15),
            ContentType = contentType
        });

        return Task.FromResult(url);
    }
}