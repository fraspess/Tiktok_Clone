using System.ComponentModel.DataAnnotations;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Application.Dtos.User;
using Application.Interfaces;
using Application.Options;
using Domain.Exceptions;
using Microsoft.Extensions.Options;
using ValidationException = Domain.Exceptions.ValidationException;

namespace Infrastructure.Services.Storage;

internal class S3StorageService(IAmazonS3 s3Client, IOptions<AwsS3Options> options) : IStorageService
{
    private readonly AwsS3Options _options = options.Value;

    public string GetVideoThumbnail(Guid videoId)
    {
        return $"{_options.CdnBaseUrl}/uploads/processed/{videoId}/thumbnail.jpg";
    }

    public string GetVideoEntryFile(Guid videoId)
    {
        return $"{_options.CdnBaseUrl}/uploads/processed/{videoId}/master.m3u8";
    }

    public AvatarDto GetUserAvatar(Guid userId)
    {
        return new AvatarDto
        {
            Small = $"{_options.CdnBaseUrl}/avatars/{userId}/small.webp?v={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
            Medium =
                $"{_options.CdnBaseUrl}/avatars/{userId}/medium.webp?v={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
            Large = $"{_options.CdnBaseUrl}/avatars/{userId}/large.webp?v={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}"
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
            throw new BadRequestException("Тільки відео файли дозволені");

        var url = s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
        {
            BucketName = _options.BucketName,
            Key = $"uploads/unprocessed/{videoId}/original",
            Verb = HttpVerb.PUT,
            Expires = DateTime.Now.AddMinutes(15),
            ContentType = contentType
        });

        var uri = new Uri(url);
        var publicUrl = new Uri(_options.CdnBaseUrl);
        var result = new UriBuilder(uri)
        {
            Scheme = publicUrl.Scheme,
            Host = publicUrl.Host,
            Port = publicUrl.IsDefaultPort ? -1 : publicUrl.Port
        }.Uri.ToString();

        return Task.FromResult(result);
    }
}