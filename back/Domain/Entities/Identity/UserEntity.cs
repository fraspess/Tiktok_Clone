using System.ComponentModel.DataAnnotations;
using Domain.Entities.Comment;
using Domain.Entities.Conversation;
using Domain.Entities.Favorite;
using Domain.Entities.Interfaces;
using Domain.Entities.Like;
using Domain.Entities.Message;
using Domain.Entities.Video;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity;

public class UserEntity : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    [MaxLength(50)] public string? LastName { get; set; }
    [MaxLength(50)] public string? FirstName { get; set; }

    [MaxLength(160)] public string? Description { get; set; }

    [MaxLength(255)] public string? Avatar { get; set; }

    public int RefreshTokenVersion { get; set; }
    
    public DateTime? LastUsernameChangedAt { get; set; }
    public bool IsDeleted { get; set; }

    public DateTime? LastConfirmationEmailSentAt { get; set; }

    public ICollection<VideoEntity> Videos { get; set; } = new List<VideoEntity>();

    public ICollection<UserFollowEntity> Following { get; set; } = new List<UserFollowEntity>();

    public ICollection<UserFollowEntity> Followers { get; set; } = new List<UserFollowEntity>();

    public ICollection<CommentEntity> Comments { get; set; } = new List<CommentEntity>();

    public ICollection<LikeEntity> Likes { get; set; } = new List<LikeEntity>();

    public virtual ICollection<UserRoleEntity>? UserRoles { get; set; }

    public ICollection<FavoriteEntity> Favorites { get; set; } = new List<FavoriteEntity>();

    public ICollection<ConversationParticipant> ConversationParticipants { get; set; } = [];

    public ICollection<MessageEntity> SentMessages { get; set; } = [];

    public ICollection<CommentLikeEntity> LikedComments { get; set; } = new List<CommentLikeEntity>();

    public Guid? BannedBy { get; set; }
    public DateTime? BannedAt { get; set; }
    public UserReportReasons? BanReason { get; set; }
    public bool IsBanned { get; set; }
}