using Domain;

namespace Application.Features.AdminPanel.GetReports;

public class AdminReportDTO
{
    public ContentTypes ContentType { get; set; }
    public Guid ContentId { get; set; }
    public string Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    public ReportUserDTO ReportedBy { get; set; }
    public ReportedContentDTO ReportedContent { get; set; }
}

public class ReportUserDTO
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string? Image { get; set; }
}

public class ReportedContentDTO
{
    public Guid Id { get; set; }
    public string? Title { get; set; }  
    public string? Thumbnail { get; set; }
}