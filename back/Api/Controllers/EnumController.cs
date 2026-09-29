using System.ComponentModel;
using System.Reflection;
using Application;
using Application.Extensions;
using Domain;
using Domain.Constants;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/enums")]
[ApiController]
public class EnumController : ControllerBase
{
    private static readonly IEnumerable<object> _contentTypes = GetEnumValues<ContentTypes>();

    private static readonly IEnumerable<object>
        _videoReportReasons = GetEnumValues<VideoReportReasons>();

    private static readonly IEnumerable<object> _commentReportReasons =
        GetEnumValues<CommentReportReasons>();

    private static readonly IEnumerable<object> _userReportReasons = GetEnumValues<UserReportReasons>();
    
    private static readonly IEnumerable<object> _messagePrivacySettings = Enum.GetValues<MessagePrivacy>()
        .Select(value => new { id = (int)value, name = value.ToString() }).ToList();

    private static IEnumerable<object> GetEnumValues<T>()
        where T : struct, Enum
    {
        return Enum.GetValues<T>()
            .Select(e => new { name = e.ToString() })
            .ToList();
    }

    [HttpGet("content-types")]
    public IActionResult GetContentTypes()
    {
        return Ok(ApiResponse<object>.Success(_contentTypes));
    }

    [HttpGet("report-reasons")]
    public IActionResult GetReportReasons([FromQuery] ContentTypes contentType)
    {
        return Ok(ApiResponse<object>.Success(contentType switch
        {
            ContentTypes.Comment => _commentReportReasons,
            ContentTypes.User => _userReportReasons,
            ContentTypes.Video => _videoReportReasons,
            _ => throw new BadRequestException(ErrorCodes.InvalidContentType, "The content type is invalid.")
        }));
    }

    [HttpGet("message-privacy")]
    public IActionResult GetMessagePrivacySettings()
    {
        return Ok(ApiResponse<object>.Success(_messagePrivacySettings));
    }
}
