using Domain.Entities.Comment;
using Domain.Entities.Conversation;
using Domain.Entities.Favorite;
using Domain.Entities.HashTags;
using Domain.Entities.Identity;
using Domain.Entities.Message;
using Domain.Entities.Report;
using Domain.Entities.Video;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IAppDbContext
{
    public DbSet<VideoEntity> Videos { get; set; }
    public DbSet<VideoLikeEntity> VideoLikes { get; set; }
    public DbSet<VideoViewEntity> VideoViews { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }
    public DbSet<CommentLikeEntity> CommentLikes { get; set; }
    public DbSet<MessageEntity> Messages { get; set; }
    public DbSet<ReportEntity> Reports { get; set; }
    public DbSet<UserFollowEntity> UserFollows { get; set; }
    public DbSet<VideoHashTagEntity> VideoHashTags { get; set; }
    public DbSet<HashTagEntity> HashTags { get; set; }
    public DbSet<ConversationEntity> Conversations { get; set; }
    
    public DbSet<VideoRepostEntity> VideoReposts { get; set; }

    public DbSet<FavoriteEntity> Favorites { get; set; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    TEntity? GetTracked<TEntity>(Func<TEntity, bool> predicate) where TEntity : class;
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
}