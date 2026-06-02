using Domain;

namespace Application.Dtos.Report;

public class ReportDTO
{
    public ContentTypes ContentType { get; set; }
    public Guid ContentId { get; set; }
    public VideoReportReasons? VideoReportReason { get; set; }
    
    public UserReportReasons? UserReportReason { get; set; }
    public CommentReportReasons? CommentReportReason { get; set; }
    public string? CustomReason { get; set; }
}