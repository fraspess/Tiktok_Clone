using Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;


namespace Infrastructure.Services.Images;

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ImageService> _logger;
    private readonly HttpClient _httpClient;

    private readonly Dictionary<int, string> _qualities = new Dictionary<int, string>
    {
        {48,"small.webp"},
        {128,"medium.webp"},
        {256, "large.webp"}
    };

    public ImageService(IWebHostEnvironment environment, ILogger<ImageService> logger, HttpClient httpClient)
    {
        _environment = environment;
        _logger = logger;
        _httpClient = httpClient;
    }


    public void DeleteImage(string imageName)
    {
        var imageFolder = Path.Combine(_environment.ContentRootPath, "images");
        var path = Path.Combine(imageFolder, imageName);
        try
        {
            File.Delete(path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Got an exception while deleting the image");
        }
    }

    private async Task SaveImagePrivate(Stream stream, Guid userId)
    {
        try
        {
            var imageFolder = Path.Combine(_environment.ContentRootPath, "images", userId.ToString());
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

        }
        catch (Exception ex)
        {
            _logger.LogError("Error while saving image. Error : {error} ", ex.Message);
        }
    }

    public async Task SaveImageAsync(IFormFile imageFile, Guid userId)
    {
        await using var stream = imageFile.OpenReadStream();
        await SaveImagePrivate(stream, userId);
    }


    public async Task SaveImageAsync(string url, Guid userId)
    {
        var httpStream = await _httpClient.GetStreamAsync(new Uri(url));
        var stream = new MemoryStream();

        await httpStream.CopyToAsync(stream);
        stream.Position = 0;
        await SaveImagePrivate(stream, userId);
    }
}