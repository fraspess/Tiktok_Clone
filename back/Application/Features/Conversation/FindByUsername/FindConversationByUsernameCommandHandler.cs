using Application.Dtos.Conversation;
using Application.Dtos.User;
using Application.Extensions;
using Application.Interfaces;
using Application.Pagination;
using MediatR;

namespace Application.Features.Conversation.FindByUsername;

public class FindConversationByUsernameCommandHandler(IAppDbContext appDbContext, ICurrentUser currentUser, IStorageService storageService) : IRequestHandler<FindConversationByUsernameCommand, PagedResult<ConversationDto>>
{
    public async Task<PagedResult<ConversationDto>> Handle(FindConversationByUsernameCommand request, CancellationToken cancellationToken)
    {
        var conversations = await appDbContext
            .Conversations
            .Where(c => c.Participants.Any(c => c.UserId == currentUser.Id!.Value))
            .ToConversationDto()
            .ToPagedResultAsync(request.PaginationSettings, cancellationToken: cancellationToken);

        return conversations.MapItems(item => new ConversationDto()
        {
            Id = item.Id,
            LastMessage = item.LastMessage,
            Participants = item.Participants.Select(p => new SimpleUserDto()
            {
                Id = p.Id,
                Username = p.Username,
                Avatar = storageService.GetUserAvatar(p.Id)
            }).ToList()
        });
    }
}