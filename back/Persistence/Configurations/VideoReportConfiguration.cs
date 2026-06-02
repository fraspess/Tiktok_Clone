using Domain.Entities.Report;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class VideoReportConfiguration : IEntityTypeConfiguration<VideoReportEntity>
{
    public void Configure(EntityTypeBuilder<VideoReportEntity> builder)
    {
        
        builder
            .HasIndex(x => new { x.VideoId, x.SenderId })
            .IsUnique();
    }
}