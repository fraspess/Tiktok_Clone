using Domain;
using Domain.Entities.Video;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class VideoHashtagConfiguration : IEntityTypeConfiguration<VideoHashTagEntity>
{
    public void Configure(EntityTypeBuilder<VideoHashTagEntity> builder)
    {
        // ── HashTags (many-to-many) ──────────────────────────
        builder
            .HasKey(vh => new { vh.VideoId, vh.HashTagId });

        builder
            .HasOne(vh => vh.Video)
            .WithMany(v => v.HashTags)
            .HasForeignKey(vh => vh.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(vh => vh.HashTag)
            .WithMany(h => h.VideoHashTags)
            .HasForeignKey(vh => vh.HashTagId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasQueryFilter(v => v.Video.Status == VideoStatus.Processed);
    }
}