using Application.Dtos.User;

namespace Application.Dtos.Conversation;

public class ConversationDto
{
    public Guid Id { get; set; }
    public List<SimpleUserDto> Participants { get; set; } = [];
    public string LastMessage { get; set; }
}