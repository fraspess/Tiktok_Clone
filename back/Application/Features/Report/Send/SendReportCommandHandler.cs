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
        var userId = user.Id!.Value;

        var contentId = await ResolveContentId(request.Dto.ContentType, request.Dto.ContentId, appDbContext, cancellationToken);

        if (await appDbContext.Reports.AnyAsync(r => r.SenderId == userId && r.ContentId == contentId,
                cancellationToken))
            throw new BadRequestException(ErrorCodes.AlreadyExists);

        appDbContext.Reports.Add(new ReportEntity
        {
            SenderId = userId,
            ContentId = contentId,
            ContentType = request.Dto.ContentType,
            Reason = request.Dto.Reason,
            OtherReason = request.Dto.CustomReason
        });
        await appDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

    private static async Task<Guid> ResolveContentId(ContentTypes contentType, string rawId,
        IAppDbContext appDbContext, CancellationToken cancellationToken)
    {
        if (contentType == ContentTypes.Video)
        {
            var videoId = await appDbContext.Videos
                .Where(v => v.ShortId == rawId)
                .Select(v => v.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (videoId == Guid.Empty) throw new NotFoundException(ErrorCodes.VideoNotFound);
            return videoId;
        }

        if (!Guid.TryParse(rawId, out var id))
            throw new BadRequestException(ErrorCodes.InvalidFileType);

        var exists = contentType switch
        {
            ContentTypes.User => await appDbContext.Set<Domain.Entities.Identity.UserEntity>().AnyAsync(u => u.Id == id, cancellationToken),
            ContentTypes.Comment => await appDbContext.Comments.AnyAsync(c => c.Id == id, cancellationToken),
            _ => false
        };
        if (!exists) throw new NotFoundException(ErrorCodes.ResourceNotFound);
        return id;
    }
}
