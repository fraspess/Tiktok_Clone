using Application.Interfaces;
using Domain;
using Domain.Entities.Report;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Report.Send;

public class SendReportCommandHandler(IAppDbContext appDbContext, ICurrentUser user)
    : IRequestHandler<SendReportCommand, Unit>
{
    public async Task<Unit> Handle(SendReportCommand request, CancellationToken cancellationToken)
    {
        var contentId = request.Dto.ContentId;
        var userId = user.Id!.Value;

        if (await appDbContext.Reports.AnyAsync(r => r.SenderId == userId && r.ContentId == contentId,
                cancellationToken))
            throw new BadRequestException(ErrorCodes.Duplicate);

        appDbContext.Reports.Add(new ReportEntity
        {
            SenderId = userId, ContentId = contentId, ContentType = request.Dto.ContentType,
            Reason = request.Dto.Reason, OtherReason = request.Dto.CustomReason
        });
        await appDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}