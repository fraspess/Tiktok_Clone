using Application.Dtos.Conversation;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Conversation.GetAll;

public class GetConversationsQueryHandler(
    IAppDbContext appDbContext,
    ICurrentUser user,
    ConversationMapper conversationMapper)
    : IRequestHandler<GetConversationsQuery, PagedResult<ConversationDto>>
{
    public async Task<PagedResult<ConversationDto>> Handle(GetConversationsQuery request,
        CancellationToken cancellationToken)
    {
        var convo = await appDbContext
            .Conversations
            .AsSplitQuery()
            .Include(c => c.Participants)
            .ThenInclude(f => f.User)
            .Where(c => c.Participants.Any(p => p.UserId == user.Id))
            .OrderByDescending(x => x.CreatedAt)
            .ToPagedResultAsync(request.PaginationSettings, cancellationToken: cancellationToken);

        var result = convo.MapItems(conversationMapper.ToDto);
        return result;
    }
}