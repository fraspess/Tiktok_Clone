using Application.Dtos.User;
using Domain;

namespace Application.Dtos.Video;

public class MyVideoDto
{
    public Guid Id { get; set; }
    public string VideoUrl { get; set; }

    public string Description { get; set; } = string.Empty;

    public List<string> HashTags { get; set; } = new();

    public string ThumbnailUrl { get; set; } = string.Empty;
    public int LikeCount { get; set; }

    public int CommentsCount { get; set; }

    public int FavoriteCount { get; set; }

    public bool IsFavorited { get; set; }
    public bool IsLiked { get; set; }

    public UserAuthorDto? Author { get; set; }
    public VideoStatus Status { get; set; }
    public bool IsProcessed => Status == VideoStatus.Processed;

    public int ProccessedInPercents { get; set; }

    public DateTime CreatedAt { get; set; }
}