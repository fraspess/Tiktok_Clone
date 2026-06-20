using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IImageService
{
    public Task SaveImageAsync(IFormFile imageFile, Guid userId);
    public Task SaveImageAsync(string url, Guid userId);
}