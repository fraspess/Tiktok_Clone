using Application.Dtos.Message;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Conversation.GetMessages;

public class GetConversationMessagesQueryHandler(
    IAppDbContext appDbContext,
    MessageMapper messageMapper,
    ICurrentUser user)
    : IRequestHandler<GetConversationMessagesQuery, PagedResult<MessageDto>>
{
    public async Task<PagedResult<MessageDto>> Handle(GetConversationMessagesQuery request,
        CancellationToken cancellationToken)
    {
        var conversation = await appDbContext
                               .Conversations
                               .Include(c => c.Participants)
                               .FirstOrDefaultAsync(c => c.Id == request.ConversationId, cancellationToken)
                           ?? throw new NotFoundException(ErrorCodes.ResourceNotFound);

        if (conversation.Participants.All(p => p.UserId != user.Id))
            throw new NotAllowedException(ErrorCodes.Forbidden);

        var messages = await appDbContext
            .Messages
            .Where(m => m.ConversationId == request.ConversationId)
            .Include(c => c.Sender)
            .OrderByDescending(m => m.CreatedAt)
            .ToPagedResultAsync(request.Settings, cancellationToken);

        var result = messages.MapItems(messageMapper.ToDto);
        return result;
    }
}