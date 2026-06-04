using Application.Dtos.Message;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Message.Get
{
    public class GetMessagesQueryHandler(IAppDbContext appDbContext, MessageMapper _mapper)
        : IRequestHandler<GetMessagesQuery, PagedResult<MessageDto>>
    {
        public async Task<PagedResult<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            var conversationExists = await appDbContext
                .Conversations
                .AnyAsync(u => u.Id == request.ConversationId, cancellationToken: cancellationToken);
            if (!conversationExists) throw new NotFoundException("Чат не знайдено");

            var messages = await appDbContext
                .Messages
                .Where(m => m.ConversationId == request.ConversationId)
                .OrderByDescending(m => m.CreatedAt)
                .ToPagedResultAsync(request.Settings);
            
            var result = messages.MapItems(_mapper.ToDto);
            return result;
        }
    }
}