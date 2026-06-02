namespace Application.Dtos.Message
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public string SenderUsername { get; set; } = String.Empty;
        public string SenderAvatarUrl { get; set; } = String.Empty;
        public string Content { get; set; } = String.Empty;
        public DateTime CreatedAt { get; set; }

        public bool IsOwn { get; set; }
    }
}