using MediatR;

namespace Application.Features.Report.GetVideoReasons;

public record GetVideoReportReasonsCommand() : IRequest<List<string>>;