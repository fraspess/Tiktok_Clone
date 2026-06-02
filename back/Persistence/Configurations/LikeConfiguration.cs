using Domain.Entities.Like;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class LikeConfiguration : IEntityTypeConfiguration<LikeEntity>
{
    public void Configure(EntityTypeBuilder<LikeEntity> builder)
    {
        // ── Likes (many-to-many) ─────────────────────────────
        builder
            .HasOne(l => l.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(l => l.Video)
            .WithMany(v => v.Likes)
            .HasForeignKey(l => l.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(l => new { l.UserId, l.VideoId })
            .IsUnique();
    }
}