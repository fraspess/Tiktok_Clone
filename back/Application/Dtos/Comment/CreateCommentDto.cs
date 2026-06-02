namespace Application.Dtos.Comment
{
    public class CreateCommentDto
    {
        public string Text { get; set; } = String.Empty;

        public Guid VideoId { get; set; }
        public Guid? ParentCommentId { get; set; }
    }
}