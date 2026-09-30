using Application.Dtos.User;

namespace Application.Dtos.Comment;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    
    public AvatarDto AvatarUrl { get; set; }
    public string AuthorUsername { get; set; } = string.Empty;
    public bool IsLiked { get; set; }

    public int LikesCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsOwn { get; set; }
}