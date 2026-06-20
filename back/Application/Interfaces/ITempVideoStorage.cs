using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface ITempVideoStorage
{
    public Task<string> SaveVideoAsync(IFormFile file);
}