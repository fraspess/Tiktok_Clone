using Domain.Entities.Comment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CommentLikeConfiguration : IEntityTypeConfiguration<CommentLikeEntity>
{
    public void Configure(EntityTypeBuilder<CommentLikeEntity> builder)
    {
        builder
            .HasKey(c => new { c.UserId, c.CommentId });

        builder
            .HasOne(p => p.Comment)
            .WithMany(c => c.CommentLikes)
            .HasForeignKey(p => p.CommentId);

        builder
            .HasOne(p => p.User)
            .WithMany(u => u.LikedComments)
            .HasForeignKey(u => u.UserId);

        builder
            .HasIndex(x => new { x.UserId, x.CommentId })
            .IsUnique(); 
    }
}