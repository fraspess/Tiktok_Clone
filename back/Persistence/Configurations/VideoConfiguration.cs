using Domain;
using Domain.Entities.Video;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class VideoConfiguration : IEntityTypeConfiguration<VideoEntity>
{
    public void Configure(EntityTypeBuilder<VideoEntity> builder)
    {
        builder
            .HasOne(v => v.Author)
            .WithMany(u => u.Videos)
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        builder
            .HasQueryFilter(v => 
                v.Status == VideoStatus.Processed && 
                !v.IsBanned && 
                !v.IsDeleted);
    }
}