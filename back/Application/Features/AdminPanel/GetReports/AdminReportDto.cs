using Domain;

namespace Application.Features.AdminPanel.GetReports;

public class AdminReportDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public ReportStatus Status { get; set; }
    public string? Reason { get; set; }

    public ReportUserDto ReportedBy { get; set; }
    public ReportedContentDto ReportedContent { get; set; }
}