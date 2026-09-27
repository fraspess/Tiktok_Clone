using Application.Dtos.Conversation;
using Application.Dtos.User;
using Application.Interfaces;
using Domain.Entities.Conversation;
using Riok.Mapperly.Abstractions;
using Serilog;

namespace Application.Mapper;

[Mapper]
public partial class ConversationMapper(IStorageService storageService)
{
    public partial ConversationDto ToDto(ConversationEntity source);

    [MapProperty(nameof(ConversationParticipant.UserId), nameof(SimpleUserDto.Id))]
    [MapProperty(nameof(ConversationParticipant.UserId), nameof(SimpleUserDto.Avatar), Use = nameof(AvatarUrl))]
    private partial SimpleUserDto ParticipantToSimpleUserDto(ConversationParticipant source);


    [UserMapping(Default = false)]
    private object AvatarUrl(Guid userId)
    {
        return storageService.GetUserAvatar(userId);
    }
}