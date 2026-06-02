using Domain.Entities.Comment;
using Domain.Entities.Conversation;
using Domain.Entities.Favorite;
using Domain.Entities.HashTags;
using Domain.Entities.Identity;
using Domain.Entities.Like;
using Domain.Entities.Message;
using Domain.Entities.Report;
using Domain.Entities.Video;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<
    UserEntity,
    RoleEntity,
    Guid,
    IdentityUserClaim<Guid>,
    UserRoleEntity,
    IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>,
    IdentityUserToken<Guid>
>(options)
{
    public DbSet<VideoEntity> Videos { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }
    public DbSet<MessageEntity> Messages { get; set; }
    public DbSet<ReportEntity> Reports { get; set; }
    public DbSet<UserFollowEntity> UserFollows { get; set; }
    public DbSet<VideoHashTagEntity> VideoHashTags { get; set; }
    public DbSet<HashTagEntity> HashTags { get; set; }
    public DbSet<ConversationEntity> Conversations { get; set; }

    public DbSet<FavoriteEntity> Favorites { get; set; }

    public DbSet<LikeEntity> Likes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

    }
}