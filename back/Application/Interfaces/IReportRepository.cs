using Application.Features.AdminPanel.GetReports;
using Application.Pagination;
using Domain;
using Domain.Entities.Report;

namespace Application.Interfaces;

public interface IReportRepository : IGenericRepository<ReportEntity>
{
    Task<bool> ExistsAsync(Guid senderId, Guid contentId, ContentTypes contentType);
    public Task<PagedResult<AdminReportDTO>> GetReports(ContentTypes contentType, PaginationSettings pagination);
}