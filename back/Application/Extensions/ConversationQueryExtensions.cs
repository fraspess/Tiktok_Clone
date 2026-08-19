using Application.Dtos.Conversation;
using Application.Dtos.User;
using Domain.Entities.Conversation;

namespace Application.Extensions;

public static class ConversationQueryExtensions
{
    public static IQueryable<ConversationDto> ToConversationDto(this IQueryable<ConversationEntity> conversations)
    {
        return conversations.Select(c => new ConversationDto()
        {
            Id = c.Id,
            LastMessage = c.Messages.OrderByDescending(m => m.CreatedAt).Select(m => m.Content).FirstOrDefault()!,
            Participants = c.Participants
                .Select(p => new SimpleUserDto()
                {
                    Id = p.UserId,
                    Username = p.User.UserName!
                })
                .ToList()
        });
    }
}