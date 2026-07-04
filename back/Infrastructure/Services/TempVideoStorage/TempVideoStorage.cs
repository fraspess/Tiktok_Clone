using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.TempVideoStorage;

internal class TempVideoStorage : ITempVideoStorage
{
    private readonly string _tempPath = Path.Combine(Directory.GetCurrentDirectory(), "temp");

    public async Task<string> SaveVideoAsync(IFormFile file)
    {
        Directory.CreateDirectory(_tempPath);

        var filePath = Path.Combine(_tempPath, $"{Guid.NewGuid().ToString()}.mp4");
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        Console.WriteLine($"Saved to: {stream.Name}, exists: {File.Exists(stream.Name)}");
        return stream.Name;
    }
}