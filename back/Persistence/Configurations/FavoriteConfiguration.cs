using Domain.Entities.Favorite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class FavoriteConfiguration : IEntityTypeConfiguration<FavoriteEntity>
{
    public void Configure(EntityTypeBuilder<FavoriteEntity> builder)
    {
        // favorites many-to-many
        builder
            .HasOne(f => f.Video)
            .WithMany(v => v.Favorites)
            .HasForeignKey(f => f.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(f => new { f.UserId, f.VideoId })
            .IsUnique();
    }
}