using Application.Dtos.Message;
using Application.Interfaces;
using Domain.Entities.Message;
using Riok.Mapperly.Abstractions;

namespace Application.Mapper;

[Mapper]
public partial class MessageMapper(IStorageService storageService)
{
    [MapProperty(nameof(MessageEntity.SenderId), nameof(MessageDto.SenderId))]
    [MapProperty(nameof(MessageEntity.Sender.UserName), nameof(MessageDto.SenderUsername))]
    [MapProperty(nameof(MessageEntity.SenderId), nameof(MessageDto.SenderAvatarUrl), Use = nameof(GetUserAvatar))]
    public partial MessageDto ToDto(MessageEntity source);

    [UserMapping(Default = false)]
    private object GetUserAvatar(Guid id) => storageService.GetUserAvatar(id);
}