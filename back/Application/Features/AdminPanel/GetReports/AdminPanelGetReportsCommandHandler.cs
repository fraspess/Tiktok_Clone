using Application.Extensions;
using Application.Interfaces;
using Application.Pagination;
using Domain;
using Domain.Entities.Identity;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.AdminPanel.GetReports;

internal class AdminPanelGetReportsCommandHandler(
    IAppDbContext appDbContext,
    IStorageService storageService,
    UserManager<UserEntity> userManager) : IRequestHandler<AdminPanelGetReportsCommand, PagedResult<AdminReportDto>>
{
    public async Task<PagedResult<AdminReportDto>> Handle(AdminPanelGetReportsCommand request,
        CancellationToken cancellationToken)
    {
        var reports = await appDbContext
            .Reports
            .Where(r => r.ContentType == request.ReportType)
            .ToPagedResultAsync(request.PaginationSettings, cancellationToken);

        var senderIds = reports.Items.Select(r => r.SenderId).Distinct().ToList();

        var senders = await userManager
            .Users
            .Where(u => senderIds.Contains(u.Id))
            .Select(u => new { u.Id, u.UserName })
            .ToListAsync(cancellationToken);

        var senderDict = senders.ToDictionary(u => u.Id);

        var contentIds = reports.Items.Select(r => r.ContentId).Distinct().ToList();

        var contentDict = request.ReportType switch
        {
            ContentTypes.Video => contentIds.ToDictionary(id => id, id => new ReportedContentDto
            {
                Id = id,
                Thumbnail = storageService.GetVideoThumbnail(id),
                ContentUrl = storageService.GetVideoEntryFile(id)
            }),
            ContentTypes.User => (await userManager
                    .Users
                    .Where(u => contentIds.Contains(u.Id))
                    .Select(u => new { u.Id, u.UserName })
                    .ToListAsync(cancellationToken))
                .ToDictionary(u => u.Id, u => new ReportedContentDto
                {
                    Id = u.Id,
                    Title = u.UserName,
                    Thumbnail = storageService.GetUserAvatar(u.Id)
                }),
            ContentTypes.Comment => (await appDbContext
                    .Comments
                    .Where(c => contentIds.Contains(c.Id))
                    .Select(c => new { c.Id, c.Text })
                    .ToListAsync(cancellationToken))
                .ToDictionary(u => u.Id, u => new ReportedContentDto
                {
                    Id = u.Id,
                    Title = u.Text
                }),
            _ => throw new BadRequestException("Невалідний тип скарг")
        };


        return reports.MapItems(r => new AdminReportDto
        {
            Id = r.Id,
            CreatedAt = r.CreatedAt,
            Status = r.Status,
            Reason = r.OtherReason ?? (r.Reason.HasValue
                ? r.ContentType switch
                {
                    ContentTypes.Video => ((VideoReportReasons)r.Reason.Value).GetDescription(),
                    ContentTypes.User => ((UserReportReasons)r.Reason.Value).GetDescription(),
                    ContentTypes.Comment => ((CommentReportReasons)r.Reason.Value).GetDescription(),
                    _ => null
                }
                : null),
            ReportedBy = senderDict.TryGetValue(r.SenderId, out var sender)
                ? new ReportUserDto
                {
                    Id = sender.Id,
                    Username = sender.UserName,
                    Avatar = storageService.GetUserAvatar(sender.Id)
                }
                : null,
            ReportedContent = contentDict.GetValueOrDefault(r.ContentId)
        });
    }
}