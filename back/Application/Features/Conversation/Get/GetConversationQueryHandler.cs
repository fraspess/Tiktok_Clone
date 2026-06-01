using Application.Dtos.Conversation;
using Application.Interfaces;
using Application.Mapper;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Conversation.Get
{
    internal class GetConversationQueryHandler(IUnitOfWork _uow, ICurrentUser user, ConversationMapper conversationMapper)
        : IRequestHandler<GetConversationQuery, ConversationDto>
    {
        public async Task<ConversationDto> Handle(GetConversationQuery request, CancellationToken cancellationToken)
        {
            var conversation = await _uow.Conversations
                                   .GetAll()
                                   .Include(c => c.Participants)
                                   .FirstOrDefaultAsync(c => c.Id == request.ConversationId)
                               ?? throw new NotFoundException("Розмову не знайдено");

            if (conversation.Participants.All(p => p.UserId != user.Id!.Value))
                throw new NotAllowedException("Ви не маєте прав на перегляд цієї сторінки.");

            var dto =  conversationMapper.ToDto(conversation);
            return dto;
        }
    }
}