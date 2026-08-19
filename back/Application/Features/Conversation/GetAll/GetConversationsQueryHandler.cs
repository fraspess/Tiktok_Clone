using Application.Dtos.Conversation;
using Application.Dtos.User;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Conversation.GetAll;

public class GetConversationsQueryHandler(
    IAppDbContext appDbContext,
    ICurrentUser user,
    IStorageService storageService,
    ConversationMapper conversationMapper)
    : IRequestHandler<GetConversationsQuery, PagedResult<ConversationDto>>
{
    public async Task<PagedResult<ConversationDto>> Handle(GetConversationsQuery request,
        CancellationToken cancellationToken)
    {
        var convo = await appDbContext
            .Conversations
            .Where(c => c.Participants.Any(p => p.UserId == user.Id))
            .OrderByDescending(x => x.CreatedAt)
            .ToConversationDto()
            .ToPagedResultAsync(request.PaginationSettings, cancellationToken: cancellationToken);

        var mapped = convo.MapItems(item =>
        {
            return new ConversationDto()
            {
                Id = item.Id,
                LastMessage = item.LastMessage,
                Participants = item.Participants.Select(u => new SimpleUserDto()
                {
                    Id = u.Id,
                    Username = u.Username,
                    Avatar = storageService.GetUserAvatar(u.Id)
                }).ToList()
            };
        });
        return mapped;
    }
}