using Application;
using Application.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers;

public class LocalVideoUploadsController(IOptions<LocalStorageOptions> options) : ControllerBase
{
    private readonly LocalStorageOptions _options = options.Value;
    
    [HttpPut("api/videos/{videoId:guid}")]
    [RequestSizeLimit(2_000_000_000)]
    public async Task<IActionResult> UploadVideo(Guid videoId)
    {
        var dir = Path.Combine(_options.RootPath, "uploads", "unprocessed", videoId.ToString());
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, "original");
        await using var fs = new FileStream(path, FileMode.Create);
        await Request.Body.CopyToAsync(fs);
        return Ok(ApiResponse<object?>.Success(null));
    }
    
    
}