using Application.Dtos.Conversation;
using Application.Interfaces;
using AutoMapper;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Conversation.Get
{
    internal class GetConversationQueryHandler(IUnitOfWork _uow, IMapper _mapper, ICurrentUser user)
        : IRequestHandler<GetConversationQuery, ConversationDTO>
    {
        public async Task<ConversationDTO> Handle(GetConversationQuery request, CancellationToken cancellationToken)
        {
            var conversation = await _uow.Conversations
                                   .GetAll()
                                   .Include(c => c.Participants)
                                   .FirstOrDefaultAsync(c => c.Id == request.ConversationId)
                               ?? throw new NotFoundException("Розмову не знайдено");

            if (conversation.Participants.All(p => p.UserId != user.Id!.Value))
                throw new NotAllowedException("Ви не маєте прав на перегляд цієї сторінки.");

            var dto = _mapper.Map<ConversationDTO>(conversation);
            return dto;
        }
    }
}