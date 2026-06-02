using Application.Interfaces;
using Domain;
using Domain.Entities.Report;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Report.Send;

public class SendReportCommandHandler(IUnitOfWork _uow, ICurrentUser user) : IRequestHandler<SendReportCommand, Unit>
{
    public async Task<Unit> Handle(SendReportCommand request, CancellationToken cancellationToken)
    {
        var contentId = request.Dto.ContentId;
        var userId = user.Id!.Value;
        var videoReportReason = request.Dto.VideoReportReason;
        var userReportReason = request.Dto.UserReportReason;
        var commentReportReason = request.Dto.CommentReportReason;
        var otherReason = request.Dto.CustomReason;

        if (await _uow.Reports.ExistsAsync(userId, contentId, request.Dto.ContentType))
        {
            throw new BadRequestException("Ви вже надіслали скаргу на цей контент");
        }
        switch (request.Dto.ContentType)
        {
            case ContentTypes.Video:
            {
                var videoReport = new VideoReportEntity() { VideoId = contentId, SenderId = userId, Reason = videoReportReason, OtherReason = otherReason};
                await _uow.Reports.CreateAsync(videoReport);
                break;
            }
            case ContentTypes.Comment:
            {
                var commentReport = new CommentReportEntity(){CommentId = contentId, SenderId = userId, Reason = commentReportReason, OtherReason = otherReason};
                await _uow.Reports.CreateAsync(commentReport);
                break;
            }
            case ContentTypes.User:
            {
                var userReport = new UserReportEntity(){UserId = contentId, SenderId = userId, Reason = userReportReason, OtherReason = otherReason};
                await _uow.Reports.CreateAsync(userReport);
                break;
            }
            case ContentTypes.FallBack:
            default:
                throw new BadRequestException("Невідомий content type");
        }
        await _uow.SaveChangesAsync();
        return Unit.Value;
    }
}