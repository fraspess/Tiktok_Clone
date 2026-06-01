namespace Application.Dtos.Conversation
{
    public class CreateConversationDto
    {
        public List<Guid> UserIds { get; set; } = [];
    }
}