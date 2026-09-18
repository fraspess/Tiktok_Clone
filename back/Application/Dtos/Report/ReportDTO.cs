using Domain;

namespace Application.Dtos.Report;

public class ReportDTO
{
    public ContentTypes ContentType { get; set; }
    public string ContentId { get; set; } = string.Empty;
    public int? Reason { get; set; }
    public string? CustomReason { get; set; }
}