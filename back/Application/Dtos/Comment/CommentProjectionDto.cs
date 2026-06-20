namespace Application.Dtos.Comment;

public class CommentProjectionDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;

    public int RepliesCount { get; set; }

    public string OwnerUsername { get; set; } = string.Empty;

    public bool IsLiked { get; set; }

    public int LikesCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsOwn { get; set; }
}