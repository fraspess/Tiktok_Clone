using System.ComponentModel;
using System.Reflection;
using Domain;
using MediatR;

namespace Application.Features.Report.GetUserReasons;

public class GetUserReportReasonsCommandHandler : IRequestHandler<GetUserReportReasonsCommand, List<string>>
{
    public Task<List<string>> Handle(GetUserReportReasonsCommand request, CancellationToken cancellationToken)
    {
        var reasons = Enum.GetValues<UserReportReasons>()
            .Select(r => r.GetType()
                .GetField(r.ToString())!
                .GetCustomAttribute<DescriptionAttribute>()?.Description ?? r.ToString())
            .ToList();
        return Task.FromResult(reasons);
    }
}