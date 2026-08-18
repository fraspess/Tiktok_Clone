using Application.Dtos.Conversation;
using MediatR;

namespace Application.Features.Conversation.Create;

public record CreateConversationCommand(Guid UsersId) : IRequest<ConversationDto>;