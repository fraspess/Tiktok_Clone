using System.ComponentModel.DataAnnotations;
using Domain;

namespace Application.Dtos.Report;

public class ReportDTO
{
    public ContentTypes ContentType { get; set; }
    public string ContentId { get; set; } = string.Empty;
    
    [MaxLength(64)]
    public string? Reason { get; set; }
    public string? CustomReason { get; set; }
}