using Application;
using Application.Dtos.Video;
using Application.Features.Video.Delete;
using Application.Features.Video.GetById;
using Application.Features.Video.GetBySomeQuery;
using Application.Features.Video.GetFYP;
using Application.Features.Video.GetUserVideos;
using Application.Features.Video.MyVideos;
using Application.Features.Video.Upload;
using Application.Features.Video.Upload.CompleteUpload;
using Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Video
{
    [Route("api/videos")]
    [ApiController]
    public class VideoController(IMediator _mediator) : ControllerBase
    {
        /*[HttpGet("video/{fileName}")]
        public IActionResult GetVideoFileByFileName(string fileName)
        {
            var videoFile = Path.Combine(Directory.GetCurrentDirectory(), "videos", "output", fileName);
            if (!System.IO.File.Exists(videoFile))
            {
                return NotFound(ApiResponse<string>.Error("Відео не знайдено"));
            }

            var stream = System.IO.File.OpenRead(videoFile);
            return File(stream, "video/mp4", enableRangeProcessing: true, fileDownloadName: "video.mp4");
        }*/

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideoById(Guid id)
        {
            var video = await _mediator.Send(new GetVideoByIdQuery(id));
            
            return Ok(ApiResponse<VideoDto>.Success(video));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideo(Guid id)
        {
            await _mediator.Send(new DeleteVideoCommand(id));
            return Ok(ApiResponse<string>.Success("Відео успішно видалено"));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadVideo([FromBody] CreateVideoDto dto)
        {
            var url = await _mediator.Send(new UploadVideoCommand(dto));
            return Ok(ApiResponse<string>.Success(url));
        }
        
        [HttpPost("{videoId}/upload-complete")]
        public async Task<IActionResult> UploadComplete(Guid videoId)
        {
            await _mediator.Send(new CompleteUploadVideoCommand(videoId));
            return Ok(ApiResponse<object>.Success(null!));
        }


        [HttpGet("fyp")]
        public async Task<IActionResult> GetForYouPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var videos = await _mediator.Send(new GetForYouPageVideosQuery(
                new PaginationSettings { PageNumber = pageNumber, PageSize = pageSize }));
            return Ok(ApiResponse<PagedResult<VideoDto>>.Success(videos));
        }

        [HttpGet("search/{query}")]
        public async Task<IActionResult> GetVideoBySomeQuery(string query, int pageNumber = 1, int pageSize = 5)
        {
            var videos = await _mediator.Send(new GetVideosBySomeStringQuery(query,
                new PaginationSettings { PageNumber = pageNumber, PageSize = pageSize }));
            return Ok(ApiResponse<PagedResult<SimpleVideoDto>>.Success(videos));
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserVideos(Guid id, int pageNumber = 1, int pageSize = 5)
        {
            var videos = await _mediator.Send(new GetUserVideosQuery(id,
                new PaginationSettings { PageNumber = pageNumber, PageSize = pageSize }));
            return Ok(ApiResponse<PagedResult<VideoDto>>.Success(videos));
        }

        [HttpGet("user/my")]
        [Authorize]
        public async Task<IActionResult> GetMyVideos(int pageNumber = 1, int pageSize = 5)
        {
            var videos = await _mediator.Send(new GetMyVideosQuery(
                new PaginationSettings { PageNumber = pageNumber, PageSize = pageSize }));
            return Ok(ApiResponse<PagedResult<MyVideoDto>>.Success(videos));
        }
        
    }
}