using System.ComponentModel.DataAnnotations;
using Domain.Entities.Comment;
using Domain.Entities.Favorite;
using Domain.Entities.Identity;

namespace Domain.Entities.Video;

public class VideoEntity : BannableSoftDeletableEntity
{
    public string ShortId { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }

    public required Guid UserId { get; set; }

    public UserEntity? Author { get; set; }

    public int ProccessedInPercents { get; set; } // returned from microservice
    
    
    public VideoStatus Status { get; set; }
    public VideoReportReasons BanReason { get; set; }

    public ICollection<CommentEntity> Comments { get; set; } = new List<CommentEntity>();
    public int CommentCount { get; set; }
    public ICollection<VideoHashTagEntity> HashTags { get; set; } = new List<VideoHashTagEntity>();

    public ICollection<VideoLikeEntity> Likes { get; set; } = new List<VideoLikeEntity>();
    public int LikeCount { get; set; }

    public ICollection<FavoriteEntity> Favorites { get; set; } = new List<FavoriteEntity>();
    public int FavoriteCount { get; set; }
    
    public int ViewCount { get; set; }
}