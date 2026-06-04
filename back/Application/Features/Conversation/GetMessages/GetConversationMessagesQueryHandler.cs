using Application.Dtos.Message;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Conversation.GetMessages
{
    public class GetConversationMessagesQueryHandler(IAppDbContext appDbContext, MessageMapper messageMapper, ICurrentUser user)
        : IRequestHandler<GetConversationMessagesQuery, PagedResult<MessageDto>>
    {
        public async Task<PagedResult<MessageDto>> Handle(GetConversationMessagesQuery request,
            CancellationToken cancellationToken)
        {
            var conversation = await appDbContext
                                   .Conversations
                                   .Include(c => c.Participants)
                                   .FirstOrDefaultAsync(c => c.Id == request.ConversationId, cancellationToken: cancellationToken)
                               ?? throw new NotFoundException("Розмову не знайдено");

            if (conversation.Participants.All(p => p.UserId != user.Id))
                throw new NotAllowedException("Ви не маєте прав на перегляд цієї сторінки.");

            var messages = await appDbContext
                .Messages
                .Where(m => m.ConversationId == request.ConversationId)
                .OrderByDescending(m => m.CreatedAt)
                .ToPagedResultAsync(request.Settings);

            var result = messages.MapItems(messageMapper.ToDto);
            return result;
        }
    }
}