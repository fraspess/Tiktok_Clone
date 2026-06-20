using Domain;
using Domain.Entities.Report;
using Microsoft.EntityFrameworkCore;

namespace Application.Extensions;

public static class ReportQueryExtensions
{
    public static async Task<bool> ExistsAsync(this IQueryable<ReportEntity> query, Guid senderId, Guid contentId,
        CancellationToken cancellationToken = default)
    {
        return await query.AnyAsync(r =>
            r.SenderId == senderId && r.ContentId == contentId, cancellationToken);
    }
}