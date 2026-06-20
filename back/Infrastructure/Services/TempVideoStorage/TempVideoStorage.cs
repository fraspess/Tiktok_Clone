using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.TempVideoStorage;

internal class TempVideoStorage : ITempVideoStorage
{
    private string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "temp");

    public async Task<string> SaveVideoAsync(IFormFile file)
    {
        Directory.CreateDirectory(tempPath);

        var filePath = Path.Combine(tempPath, $"{Guid.NewGuid().ToString()}.mp4");
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        Console.WriteLine($"Saved to: {stream.Name}, exists: {File.Exists(stream.Name)}");
        return stream.Name;
    }
}