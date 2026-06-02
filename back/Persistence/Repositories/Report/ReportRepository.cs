using Application.Extensions;
using Application.Features.AdminPanel.GetReports;
using Application.Interfaces;
using Application.Pagination;
using Domain;
using Domain.Entities.Report;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories.Report;

internal class ReportRepository(AppDbContext _context, IStorageService storageService)
    : GenericRepository<ReportEntity>(_context), IReportRepository
{
    public async Task<bool> ExistsAsync(Guid senderId, Guid contentId, ContentTypes contentType)
    {
        return contentType switch
        {
            ContentTypes.Video => await _context.Set<VideoReportEntity>()
                .AnyAsync(r => r.SenderId == senderId && r.VideoId == contentId),
            ContentTypes.Comment => await _context.Set<CommentReportEntity>()
                .AnyAsync(r => r.SenderId == senderId && r.CommentId == contentId),
            ContentTypes.User => await _context.Set<UserReportEntity>()
                .AnyAsync(r => r.SenderId == senderId && r.UserId == contentId),
            _ => false
        };
    }

    public async Task<PagedResult<AdminReportDTO>> GetReports(ContentTypes contentType, PaginationSettings pagination)
    {
        return contentType switch
        {
            ContentTypes.Video => await _context.Set<VideoReportEntity>()
                .Select(r => new AdminReportDTO
                {
                    ContentType = ContentTypes.Video,
                    ContentId = r.VideoId,
                    Reason = r.OtherReason ?? r.Reason.GetDescription(),
                    CreatedAt = r.CreatedAt,
                    ReportedBy = new ReportUserDTO
                    {
                        Id = r.Sender.Id,
                        Username = r.Sender.UserName,
                        Image = storageService.GetVideoThumbnail(r.VideoId)
                    },
                    ReportedContent = new ReportedContentDTO
                    {
                        Id = r.Video.Id,
                        Title = r.Video.Description,
                        Thumbnail = null
                    }
                })
                .ToPagedResultAsync(pagination),

            ContentTypes.User => await _context.Set<UserReportEntity>()
                .Select(r => new AdminReportDTO
                {
                    ContentType = ContentTypes.User,
                    ContentId = r.UserId,
                    Reason = r.OtherReason ?? r.Reason.GetDescription(),
                    CreatedAt = r.CreatedAt,
                    ReportedBy = new ReportUserDTO
                    {
                        Id = r.Sender.Id,
                        Username = r.Sender.UserName,
                        Image = storageService.GetUserAvatar(r.SenderId)
                    },
                    ReportedContent = new ReportedContentDTO
                    {
                        Id = r.User.Id,
                        Title = r.User.UserName,
                        Thumbnail = storageService.GetUserAvatar(r.UserId)
                    }
                })
                .ToPagedResultAsync(pagination),

            ContentTypes.Comment => await _context.Set<CommentReportEntity>()
                .Select(r => new AdminReportDTO
                {
                    ContentType = ContentTypes.Comment,
                    ContentId = r.CommentId,
                    Reason = r.OtherReason ?? r.Reason.GetDescription(),
                    CreatedAt = r.CreatedAt,
                    ReportedBy = new ReportUserDTO
                    {
                        Id = r.Sender.Id,
                        Username = r.Sender.UserName,
                        Image = storageService.GetUserAvatar(r.SenderId)
                    },
                    ReportedContent = new ReportedContentDTO
                    {
                        Id = r.Comment.Id,
                        Title = r.Comment.Text,
                        Thumbnail = null
                    }
                })
                .ToPagedResultAsync(pagination),
        };
    }
}