using System.ComponentModel;
using System.Reflection;
using Application;
using Application.Extensions;
using Domain;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/enums")]
[ApiController]
public class EnumController : ControllerBase
{
    private static readonly IEnumerable<object> _contentTypes = GetEnumValues<ContentTypes>();
    private static readonly IEnumerable<object> _videoReportReasons = GetEnumValuesWithDescription<VideoReportReasons>();
    private static readonly IEnumerable<object> _commentReportReasons = GetEnumValuesWithDescription<CommentReportReasons>();
    private static readonly IEnumerable<object> _userReportReasons = GetEnumValuesWithDescription<UserReportReasons>();
    
    private static IEnumerable<object> GetEnumValues<T>() 
        where T : struct, Enum
        => Enum.GetValues<T>()
            .Select(e => new { id = Convert.ToInt32(e), name = e.ToString() })
            .ToList();

    private static IEnumerable<object> GetEnumValuesWithDescription<T>()
        where T : struct, Enum
        => Enum.GetValues<T>()
            .Select(e => new
            {
                id = Convert.ToInt32(e),
                name = e.ToString(),
                description = e.GetDescription() 
            })
            .ToList();
        
    [HttpGet("content-types")]
    public IActionResult GetContentTypes() => Ok(ApiResponse<object>.Success(_contentTypes));

    [HttpGet("report-reasons")]
    public IActionResult GetReportReasons(ContentTypes contentType)
        => Ok(ApiResponse<object>.Success(contentType switch
        {
            ContentTypes.Comment => _commentReportReasons,
            ContentTypes.User => _userReportReasons,
            ContentTypes.Video => _videoReportReasons,
             _ => throw new BadRequestException("Невідомий contentType")
        }));
}