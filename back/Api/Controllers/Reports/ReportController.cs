using Application;
using Application.Dtos.Report;
using Application.Extensions;
using Application.Features.Report.GetVideoReasons;
using Application.Features.Report.Send;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Reports;

[Route("api/reports")]
[ApiController]
[Authorize]
public class ReportController(IMediator mediator) : ControllerBase
{
    [HttpGet("video-reasons")]
    public async Task<IActionResult> GetVideoReportReasons()
    {
        var reasons = await mediator.Send(new GetVideoReportReasonsCommand());
        return Ok(ApiResponse<object>.Success(reasons));
    }

    [HttpGet("user-reasons")]
    public async Task<IActionResult> GetUserReportReasons()
    {
        var reasons = await mediator.Send(new GetVideoReportReasonsCommand());
        return Ok(ApiResponse<object>.Success(reasons));
    }

    [HttpPost]
    public async Task<IActionResult> SendReport([FromBody] ReportDTO dto)
    {
        await mediator.Send(new SendReportCommand(dto));
        return Ok(ApiResponse<object>.Success(null!, "Успішно відправлено скаргу!"));
    }
}