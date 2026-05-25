using Application.Dtos.Conversation;
using Application.Pagination;
using MediatR;

namespace Application.Features.Conversation.GetAll
{
    public record GetConversationsQuery(PaginationSettings PaginationSettings)
        : IRequest<PagedResult<ConversationDTO>>;
}