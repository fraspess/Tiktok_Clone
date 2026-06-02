using Application.Dtos.Message;
using Application.Pagination;
using MediatR;

namespace Application.Features.Message.Get
{
    public record GetMessagesQuery(Guid ConversationId, PaginationSettings Settings)
        : IRequest<PagedResult<MessageDto>>;
}