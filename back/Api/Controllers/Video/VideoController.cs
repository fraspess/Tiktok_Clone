using Api.RateLimiting;
using Application;
using Application.Dtos.Video;
using Application.Features.Video.Delete;
using Application.Features.Video.Favorite;
using Application.Features.Video.GetById;
using Application.Features.Video.GetBySomeQuery;
using Application.Features.Video.GetFavorites;
using Application.Features.Video.GetFollowingFyp;
using Application.Features.Video.GetFyp;
using Application.Features.Video.GetUserVideos;
using Application.Features.Video.Like;
using Application.Features.Video.MyVideos;
using Application.Features.Video.Repost;
using Application.Features.Video.Unfavorite;
using Application.Features.Video.Unlike;
using Application.Features.Video.UnRepost;
using Application.Features.Video.Upload;
using Application.Features.Video.Upload.CompleteUpload;
using Application.Features.Video.View;
using Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Video;

[Route("api/videos")]
[ApiController]
public class VideoController(IMediator _mediator, Application.Interfaces.IAppDbContext db) : ControllerBase
{
    private async Task<Guid> ResolveRepostId(string id)
    {
        var guid = Guid.TryParse(id, out var parsed) ? parsed : Guid.Empty;
        var videoId = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
            db.Videos.Where(v => v.ShortId == id || v.Id == guid).Select(v => v.Id));
        if (videoId == Guid.Empty) throw new Domain.Exceptions.NotFoundException(Domain.Constants.ErrorCodes.VideoNotFound);
        return videoId;
    }

    /*[HttpGet("video/{fileName}")]
    public IActionResult GetVideoFileByFileName(string fileName)
    {
        var videoFile = Path.Combine(Directory.GetCurrentDirectory(), "videos", "output", fileName);
        if (!System.IO.File.Exists(videoFile))
        {
            return NotFound(ApiResponse<string>.Error(ErrorCodes.VideoNotFound, "Video not found."));
        }

        var stream = System.IO.File.OpenRead(videoFile);
        return File(stream, "video/mp4", enableRangeProcessing: true, fileDownloadName: "video.mp4");
    }*/

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVideoById(string id)
    {
        var video = await _mediator.Send(new GetVideoByIdQuery(id));

        return Ok(ApiResponse<VideoDto>.Success(video));
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVideo(string id)
    {
        await _mediator.Send(new DeleteVideoCommand(id));
        return Ok(ApiResponse<string>.Success("Відео успішно видалено"));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UploadVideo([FromBody] UploadVideoCommand command)
    {
        var url = await _mediator.Send(command);
        // {Url = url, VideoId = videoId}
        return Ok(ApiResponse<object>.Success(url));
    }

    [HttpPost("upload-complete")]
    [Authorize]
    public async Task<IActionResult> UploadComplete([FromBody] CompleteUploadVideoCommand command)
    {       
        await _mediator.Send(command);
        return Ok(ApiResponse<object>.Success(null!));
    }


    [HttpGet("fyp")]
    public async Task<IActionResult> GetForYouPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var videos = await _mediator.Send(new GetForYouPageVideosQuery(
            new PaginationSettings { PageNumber = pageNumber, PageSize = pageSize }));
        return Ok(ApiResponse<PagedResult<VideoDto>>.Success(videos));
    }

    [HttpGet("fyp/following")]
    [Authorize]
    public async Task<IActionResult> GetFollowingVideos([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(ApiResponse<object>.Success(await _mediator.Send(new GetFollowingFypCommand(new PaginationSettings
            { PageNumber = pageNumber, PageSize = pageSize }))));
    }

    [HttpGet("search/{query}")]
    public async Task<IActionResult> GetVideoBySomeQuery(string query, int pageNumber = 1, int pageSize = 10)
    {
        var videos = await _mediator.Send(new GetVideosBySomeStringQuery(query,
            new PaginationSettings { PageNumber = pageNumber, PageSize = pageSize }));
        return Ok(ApiResponse<PagedResult<SimpleVideoDto>>.Success(videos));
    }

    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetUserVideos(Guid id, int pageNumber = 1, int pageSize = 10)
    {
        var videos = await _mediator.Send(new GetUserVideosQuery(id,
            new PaginationSettings { PageNumber = pageNumber, PageSize = pageSize }));
        return Ok(ApiResponse<PagedResult<VideoDto>>.Success(videos));
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMyVideos(int pageNumber = 1, int pageSize = 10)
    {
        var videos = await _mediator.Send(new GetMyVideosQuery(
            new PaginationSettings { PageNumber = pageNumber, PageSize = pageSize }));
        return Ok(ApiResponse<PagedResult<MyVideoDto>>.Success(videos));
    }

    [Authorize]
    [HttpPost("{videoId}/like")]
    public async Task<IActionResult> ToggleLike(string videoId)
    {
        await _mediator.Send(new LikeVideoCommand(videoId));
        return Ok(ApiResponse<object>.Success(null!, null));
    }

    [Authorize]
    [HttpDelete("{videoId}/like")]
    public async Task<IActionResult> Unlike(string videoId)
    {
        await _mediator.Send(new UnlikeVideoCommand(videoId));
        return Ok(ApiResponse<object>.Success(null!));
    }

    [HttpPost("{videoId}/view")]
    public async Task<IActionResult> ViewVideo(string videoId)
    {   
        await _mediator.Send(new ViewVideoCommand(videoId));
        return Ok(ApiResponse<object>.Success(null!));
    }
    
    [HttpPost("{videoId}/favorite")]
    [Authorize]
    public async Task<IActionResult> Favorite(string videoId)
    {
        await _mediator.Send(new FavoriteVideoCommand(videoId));
        return Ok(ApiResponse<object>.Success(null!, null));
    }

    [HttpDelete("{videoId}/favorite")]
    [Authorize]
    public async Task<IActionResult> Unfavorite(string videoId)
    {
        await _mediator.Send(new UnfavoriteCommand(videoId));
        return Ok(ApiResponse<object>.Success(null!));
    }

    [HttpPost("{videoId}/repost")]
    [Authorize]
    public async Task<IActionResult> Repost(string videoId)
    {
        await _mediator.Send(new RepostVideoCommand(await ResolveRepostId(videoId)));
        return Ok(ApiResponse<object>.Success(null!));
    }

    [HttpDelete("{videoId}/repost")]
    [Authorize]
    public async Task<IActionResult> Unrepost(string videoId)
    {
        await _mediator.Send(new UnRepostVideoCommand(await ResolveRepostId(videoId)));
        return Ok(ApiResponse<object>.Success(null!));
    }
}
