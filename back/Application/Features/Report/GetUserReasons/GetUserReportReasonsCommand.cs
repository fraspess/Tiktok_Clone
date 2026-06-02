using MediatR;

namespace Application.Features.Report.GetUserReasons;

public record GetUserReportReasonsCommand() : IRequest<List<string>>;