using Domain;

namespace Application.Dtos.Report;

public class ReportDTO
{
    public ContentTypes ContentType { get; set; }
    public Guid ContentId { get; set; }
    public int? Reason { get; set; }
    public string? CustomReason { get; set; }
}