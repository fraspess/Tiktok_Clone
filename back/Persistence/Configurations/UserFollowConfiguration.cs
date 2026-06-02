using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class UserFollowConfiguration : IEntityTypeConfiguration<UserFollowEntity>
{
    public void Configure(EntityTypeBuilder<UserFollowEntity> builder)
    {
        builder
            .HasKey(uf => new { uf.FollowerId, uf.FollowingId });

        builder
            .HasOne(uf => uf.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(uf => uf.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(uf => uf.Following)
            .WithMany(u => u.Followers)
            .HasForeignKey(uf => uf.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}