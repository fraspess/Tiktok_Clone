using Application.Extensions;
using Application.Interfaces;
using Application.Pagination;
using Domain;
using MediatR;

namespace Application.Features.AdminPanel.GetReports;

internal class AdminPanelGetReportsCommandHandler(IUnitOfWork _uow) : IRequestHandler<AdminPanelGetReportsCommand, PagedResult<AdminReportDTO>>
{
    public async Task<PagedResult<AdminReportDTO>> Handle(AdminPanelGetReportsCommand request, CancellationToken cancellationToken)
    {
        return await _uow.Reports.GetReports(request.ReportType, request.PaginationSettings);
    }
}