using Domain.Entities.Video;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class VideoViewConfiguration : IEntityTypeConfiguration<VideoViewEntity>
{
    public void Configure(EntityTypeBuilder<VideoViewEntity> builder)
    {
        builder
            .HasIndex(x => new { x.UserId, x.VideoId, x.ViewedAt });
    }
}