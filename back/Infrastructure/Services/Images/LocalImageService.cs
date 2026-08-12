using Application.Interfaces;
using Application.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Services.Images;

public class LocalImageService(
    ILogger<LocalImageService> logger,
    HttpClient httpClient,
    IOptions<LocalStorageOptions> options)
    : IImageService
{
    private readonly LocalStorageOptions _options = options.Value;

    private readonly Dictionary<int, string> _qualities = new()
    {
        { 48, "small.webp" },
        { 128, "medium.webp" },
        { 256, "large.webp" }
    };

    private async Task SaveImagePrivate(Stream stream, Guid userId)
    {
        try
        {
            var destDir = Path.Combine(_options.RootPath, "avatars", userId.ToString());
            Directory.CreateDirectory(destDir);

            using var image = await Image.LoadAsync(stream);

            foreach (var (quality, name) in _qualities)
                await image.Clone(x => x.Resize(quality, quality))
                    .SaveAsWebpAsync(Path.Combine(destDir, name));
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