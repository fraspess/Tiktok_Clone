using Application.Dtos.Conversation;
using Application.Interfaces;
using Application.Mapper;
using Domain.Entities.Conversation;
using Domain.Entities.Identity;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Conversation.Create;

public class CreateConversationCommandHandler(
    IAppDbContext appDbContext,
    ConversationMapper mapper,
    UserManager<UserEntity> _userManager,
    ICurrentUser currentUser)
    : IRequestHandler<CreateConversationCommand, ConversationDto>
{
    public async Task<ConversationDto> Handle(CreateConversationCommand request,
        CancellationToken cancellationToken)
    {
        var participants = request.UsersIds;
        var currentUserId = currentUser.Id!.Value;

        if (!participants.Contains(currentUserId)) participants.Add(currentUserId);

        var existingConversation = await appDbContext
            .Conversations
            .Include(c => c.Participants)
            .Where(c => c.Participants.Count == participants.Count &&
                        c.Participants.All(p => participants.Contains(p.UserId)))
            .FirstOrDefaultAsync(cancellationToken);

        if (existingConversation is not null) return mapper.ToDto(existingConversation);

        foreach (var participant in participants)
            if (!await _userManager.Users.AnyAsync(u => u.Id == participant, cancellationToken))
                throw new NotFoundException("Користувача не знайдено. Спробуйте створити бесіду ще раз.");

        var conversation = new ConversationEntity
        {
            Participants = participants.Select(id => new ConversationParticipant
            {
                UserId = id
            }).ToList()
        };

        await appDbContext.Conversations.AddAsync(conversation, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        return mapper.ToDto(conversation);
    }
}