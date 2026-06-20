using System.ComponentModel.DataAnnotations;
using Domain.Entities.Identity;

namespace Domain.Entities.Report;

public class ReportEntity : AuditableEntity
{
    public Guid SenderId { get; set; }
    public UserEntity Sender { get; init; } = null!;

    public int? Reason { get; set; }
    [MaxLength(255)] public string? OtherReason { get; set; }

    public ContentTypes ContentType { get; set; }
    public Guid ContentId { get; set; }

    public ReportStatus Status { get; set; } = ReportStatus.Pending;
}