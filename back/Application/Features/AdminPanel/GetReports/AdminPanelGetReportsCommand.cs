using Application.Dtos.Report;
using Application.Pagination;
using Domain;
using MediatR;

namespace Application.Features.AdminPanel.GetReports;

public record AdminPanelGetReportsCommand(ContentTypes ReportType, PaginationSettings PaginationSettings) : IRequest<PagedResult<AdminReportDTO>>;
