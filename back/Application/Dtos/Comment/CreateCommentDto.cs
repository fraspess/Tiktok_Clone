namespace Application.Dtos.Comment;

public class CreateCommentDto
{
    public string Text { get; set; } = string.Empty;

    public string VideoId { get; set; }
    public Guid? ParentCommentId { get; set; }
}