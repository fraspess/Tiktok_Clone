using Application.Dtos.Report;
using MediatR;

namespace Application.Features.Report.Send;

public record SendReportCommand(ReportDTO Dto) : IRequest<Unit>;