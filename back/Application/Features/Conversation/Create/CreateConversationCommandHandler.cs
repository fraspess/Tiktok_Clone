using Application.Dtos.Conversation;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities.Conversation;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Conversation.Create
{
    public class CreateConversationCommandHandler(IUnitOfWork _uow, IMapper _mapper, IUserService _userManager, ICurrentUser currentUser)
        : IRequestHandler<CreateConversationCommand, ConversationDTO>
    {
        public async Task<ConversationDTO> Handle(CreateConversationCommand request,
            CancellationToken cancellationToken)
        {
            var participants = request.UsersIds;
            var currentUserId = currentUser.Id!.Value;

            if (!participants.Contains(currentUserId))
            {
                participants.Add(currentUserId);
            }

            var existingConversation = await _uow.Conversations
                .GetAll()
                .Include(c => c.Participants)
                .Where(c => c.Participants.Count == participants.Count &&
                            c.Participants.All(p => participants.Contains(p.UserId)))
                .FirstOrDefaultAsync();

            if (existingConversation is not null)
            {
                return _mapper.Map<ConversationDTO>(existingConversation);
            }

            foreach (var participant in participants)
            {
                if (!await _userManager.IsExistsById(participant))
                    throw new NotFoundException("Користувача не знайдено. Спробуйте створити бесіду ще раз.");
            }

            var conversation = new ConversationEntity
            {
                Participants = participants.Select(id => new ConversationParticipant
                {
                    UserId = id,
                }).ToList()
            };

            await _uow.Conversations.CreateAsync(conversation);
            await _uow.SaveChangesAsync();

            return _mapper.Map<ConversationDTO>(conversation);
        }
    }
}