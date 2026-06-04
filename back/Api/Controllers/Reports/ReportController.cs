using Application;
using Application.Dtos.Report;
using Application.Extensions;
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
    [HttpPost]
    public async Task<IActionResult> SendReport([FromBody] ReportDTO dto)
    {
        await mediator.Send(new SendReportCommand(dto));
        return Ok(ApiResponse<object>.Success(null!, "Успішно відправлено скаргу!"));
    }
}