using Domain.Entities.Report;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class UserReportConfiguration : IEntityTypeConfiguration<UserReportEntity>
{
    public void Configure(EntityTypeBuilder<UserReportEntity> builder)
    {
        builder
            .HasIndex(x => new { x.UserId, x.SenderId })
            .IsUnique();
    }
}