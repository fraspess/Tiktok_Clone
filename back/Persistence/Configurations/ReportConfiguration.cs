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
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasDiscriminator<string>("ReportType")
            .HasValue<VideoReportEntity>("Video")
            .HasValue<UserReportEntity>("User")
            .HasValue<CommentReportEntity>("Comment");
    }
}