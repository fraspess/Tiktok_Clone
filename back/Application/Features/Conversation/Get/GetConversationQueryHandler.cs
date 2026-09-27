using Application.Dtos.Conversation;
using Application.Dtos.User;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Conversation.Get;

internal class GetConversationQueryHandler(
    IAppDbContext appDbContext,
    ICurrentUser user,
    ConversationMapper conversationMapper,
    IStorageService storageService)
    : IRequestHandler<GetConversationQuery, ConversationDto>
{
    public async Task<ConversationDto> Handle(GetConversationQuery request, CancellationToken cancellationToken)
    {
        var convo = await appDbContext
            .Conversations
            .Where(c => c.Id == request.ConversationId)
            .ToConversationDto()
            .FirstOrDefaultAsync(cancellationToken);

        if (convo == null) throw new NotFoundException(ErrorCodes.ResourceNotFound);
        if (convo.Participants.All(u => u.Id != user.Id!.Value)) throw new NotAllowedException(ErrorCodes.Forbidden);

        convo.Participants = convo.Participants.Select(p => new SimpleUserDto()
        {
            Id = p.Id,
            Username = p.Username,
            Avatar = storageService.GetUserAvatar(p.Id)
        }).ToList();
        return convo;
    }
}