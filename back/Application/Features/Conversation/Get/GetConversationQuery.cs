using Application.Dtos.Conversation;
using MediatR;

namespace Application.Features.Conversation.Get
{
    public record GetConversationQuery(Guid ConversationId) : IRequest<ConversationDto>;
}