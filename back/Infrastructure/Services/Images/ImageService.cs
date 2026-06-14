using Amazon.S3;
using Amazon.S3.Transfer;
using Application.Interfaces;
using Infrastructure.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;


namespace Infrastructure.Services.Images;

public class ImageService(ILogger<ImageService> logger, HttpClient httpClient, IAmazonS3 amazonS3, IOptions<AwsS3Options> options)
    : IImageService
{
    private readonly AwsS3Options _options = options.Value;
    private readonly Dictionary<int, string> _qualities = new Dictionary<int, string>
    {
        {48,"small.webp"},
        {128,"medium.webp"},
        {256, "large.webp"}
    };

    private async Task SaveImagePrivate(Stream stream, Guid userId)
    {
        try
        {
            var imageFolder = Path.Combine(Path.GetTempPath(), "images", userId.ToString());
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }

            using Image image = Image.Load(stream);
            
            foreach (var (quality, name) in _qualities)
            {
                await image.Clone(x => x.Resize(quality, quality))
                    .SaveAsWebpAsync(Path.Combine(imageFolder, name ));
            }

            using var transferUtility = new TransferUtility(amazonS3);

            var request = new TransferUtilityUploadDirectoryRequest()
            {
                BucketName = _options.BucketName,
                Directory = imageFolder,
                KeyPrefix = $"avatars/{userId}",
                SearchPattern = "*",
                SearchOption = SearchOption.AllDirectories
            };
            request.UploadDirectoryFileRequestEvent += (_, args) =>
            {
                args.UploadRequest.ContentType = "image/webp";
            };
            await transferUtility.UploadDirectoryAsync(request);
            Directory.Delete(imageFolder, recursive: true);
        }
        catch (Exception ex)
        {
            logger.LogError("Error while saving image. Error : {error} ", ex.Message);
        }
    }

    public async Task SaveImageAsync(IFormFile imageFile, Guid userId)
    {
        await using var stream = imageFile.OpenReadStream();
        await SaveImagePrivate(stream, userId);
    }


    public async Task SaveImageAsync(string url, Guid userId)
    {
        var httpStream = await httpClient.GetStreamAsync(new Uri(url));
        var stream = new MemoryStream();

        await httpStream.CopyToAsync(stream);
        stream.Position = 0;
        await SaveImagePrivate(stream, userId);
    }
}