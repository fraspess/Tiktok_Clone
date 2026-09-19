using Application.Interfaces;
using Domain;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.AdminPanel.MarkReportAsResolved;

public class MarkReportAsResolvedCommandHandler(IAppDbContext appDbContext) : IRequestHandler<MarkReportAsResolvedCommand, Unit>
{
    public async Task<Unit> Handle(MarkReportAsResolvedCommand request, CancellationToken cancellationToken)
    {
        var report = await appDbContext.Reports.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken: cancellationToken) ?? throw new NotFoundException(ErrorCodes.ResourceNotFound);
        report.Status = ReportStatus.Reviewed;
        appDbContext.Reports.Update(report);
        await appDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}