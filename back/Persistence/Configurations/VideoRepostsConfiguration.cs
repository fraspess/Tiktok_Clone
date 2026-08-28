using Domain.Entities.Video;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class VideoRepostsConfiguration : IEntityTypeConfiguration<VideoRepostEntity>
{
    public void Configure(EntityTypeBuilder<VideoRepostEntity> builder)
    {
        builder.HasKey(vh => new { vh.VideoId, vh.UserId });
        
        builder
            .HasOne(vh => vh.Video)
            .WithMany(v => v.Reposts)
            .HasForeignKey(vh => vh.VideoId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(vr => vr.User)
            .WithMany(u => u.Reposts)
            .HasForeignKey(vr => vr.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}