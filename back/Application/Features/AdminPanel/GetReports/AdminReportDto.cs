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

public class ReportUserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string? Avatar { get; set; }
}

public class ReportedContentDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Thumbnail { get; set; }
    public string? ContentUrl { get; set; }
}