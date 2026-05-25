using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder
            .HasIndex(u => u.UserName).IsUnique();

        builder
            .HasIndex(u => u.Email).IsUnique();

        builder
            .HasQueryFilter(u => !u.IsDeleted);
    }
}