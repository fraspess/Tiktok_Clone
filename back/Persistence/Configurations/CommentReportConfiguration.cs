using Domain.Entities.Report;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CommentReportConfiguration : IEntityTypeConfiguration<CommentReportEntity>
{
    public void Configure(EntityTypeBuilder<CommentReportEntity> builder)
    {
        builder
            .HasIndex(x => new {x.CommentId, x.SenderId})
            .IsUnique();
    }
}