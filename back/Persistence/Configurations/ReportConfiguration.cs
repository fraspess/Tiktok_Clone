using Domain;
using Domain.Entities.Report;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<ReportEntity>
{
    public void Configure(EntityTypeBuilder<ReportEntity> builder)
    {
        // ── Reports ─────────────────────────────────────────
        builder
            .HasOne(r => r.Sender)
            .WithMany()
            .HasForeignKey(r => r.SenderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasIndex(r => new { r.SenderId, r.ContentId })
            .IsUnique();

        builder
            .HasQueryFilter(r => r.Status == ReportStatus.Pending);
    }
}