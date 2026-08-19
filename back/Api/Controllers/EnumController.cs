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
        _videoReportReasons = GetEnumValuesWithDescription<VideoReportReasons>();

    private static readonly IEnumerable<object> _commentReportReasons =
        GetEnumValuesWithDescription<CommentReportReasons>();

    private static readonly IEnumerable<object> _userReportReasons = GetEnumValuesWithDescription<UserReportReasons>();
    
    private static readonly IEnumerable<object> _messagePrivacySettings = GetEnumValues<MessagePrivacy>();

    private static IEnumerable<object> GetEnumValues<T>()
        where T : struct, Enum
    {
        return Enum.GetValues<T>()
            .Select(e => new { id = Convert.ToInt32(e), name = e.ToString() })
            .Where(e => e.id != 0)
            .ToList();
    }

    private static IEnumerable<object> GetEnumValuesWithDescription<T>()
        where T : struct, Enum
    {
        return Enum.GetValues<T>()
            .Select(e => new
            {
                id = Convert.ToInt32(e),
                description = e.GetDescription()
            })
            .Where(e => e.id != 0)
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
            _ => throw new BadRequestException("Невідомий contentType")
        }));
    }

    [HttpGet("message-privacy")]
    public IActionResult GetMessagePrivacySettings()
    {
        return Ok(ApiResponse<object>.Success(_messagePrivacySettings));
    }
}