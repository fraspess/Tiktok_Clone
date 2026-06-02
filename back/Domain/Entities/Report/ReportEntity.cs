using System.ComponentModel.DataAnnotations;
using Domain.Entities.Identity;

namespace Domain.Entities.Report
{
    public abstract class ReportEntity : AuditableEntity
    {
        public Guid SenderId { get; set; }
        public UserEntity Sender { get; init; } = null!;
        [MaxLength(255)]
        public String? OtherReason { get; set; }
    }
}