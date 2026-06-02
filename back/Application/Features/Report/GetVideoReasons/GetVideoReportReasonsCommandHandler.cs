using System.ComponentModel;
using System.Reflection;
using Domain;
using MediatR;

namespace Application.Features.Report.GetVideoReasons;

internal class GetVideoReportReasonsCommandHandler : IRequestHandler<GetVideoReportReasonsCommand, List<string>>
{
    public Task<List<string>> Handle(GetVideoReportReasonsCommand request, CancellationToken cancellationToken)
    {
        var reasons = Enum.GetValues<VideoReportReasons>()
            .Select(r => r.GetType()
                .GetField(r.ToString())!
                .GetCustomAttribute<DescriptionAttribute>()?.Description ?? r.ToString())
            .ToList();
        return Task.FromResult(reasons);
    }
}