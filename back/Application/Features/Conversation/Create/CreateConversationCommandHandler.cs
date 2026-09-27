using Application.Dtos.Conversation;
using Application.Dtos.User;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Services.Message;
using Domain.Entities.Conversation;
using Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Conversation.Create;

public class CreateConversationCommandHandler(
    IAppDbContext appDbContext,
    ConversationMapper mapper,
    UserManager<UserEntity> userManager,
    ICurrentUser currentUser,
    IStorageService storageService,
    MessagePrivacyService messagePrivacy)
    : IRequestHandler<CreateConversationCommand, ConversationDto>
{
    public async Task<ConversationDto> Handle(CreateConversationCommand request,
        CancellationToken cancellationToken)
    {
        var participant = request.UsersId;
        var currentUserId = currentUser.Id!.Value;

        await messagePrivacy.EnsureCanMessageAsync(currentUserId, participant, cancellationToken);
        var existingConversation = await appDbContext.Conversations
            .Where(c => c.Participants.Any(p => p.UserId == currentUserId) &&
                        c.Participants.Any(p => p.UserId == participant))
            .ToConversationDto()
            .FirstOrDefaultAsync(cancellationToken);

        if (existingConversation is not null)
        {
            foreach (var user in existingConversation.Participants)
            {
                user.Avatar = storageService.GetUserAvatar(user.Id);
            }
            return existingConversation;
        }
        var conversation = await CreateConversationAsync(participant);
        
        await appDbContext.SaveChangesAsync(cancellationToken);
        
        return conversation;
    }

    async Task<ConversationDto> CreateConversationAsync(Guid otherId)
    {
        var conversation = new ConversationEntity()
        {
            Participants = new List<ConversationParticipant>()
            {
                new()
                {
                    UserId = otherId,
                },
                new()
                {
                    UserId = currentUser.Id!.Value
                }
            }
        };
        await appDbContext.Conversations.AddAsync(conversation);
       var convo = mapper.ToDto(conversation);
        
       var userIds = convo.Participants.Select(p => p.Id).ToList();

       var users = await userManager.Users
           .Where(u => userIds.Contains(u.Id))
           .Select(u => new
           {
               u.Id,
               u.UserName
           })
           .ToListAsync();
       
       convo.Participants = convo.Participants.Select(p =>
       {
           var user = users.FirstOrDefault(u => u.Id == p.Id);

           return new SimpleUserDto
           {
               Id = p.Id,
               Username = user?.UserName!,
               Avatar = p.Avatar
           };
       }).ToList();
       
       return convo;
    }
}
