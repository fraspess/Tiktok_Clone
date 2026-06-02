using Application.Dtos.Conversation;
using Application.Dtos.User;
using Application.Interfaces;
using Domain.Entities.Conversation;
using Domain.Entities.Identity;
using Riok.Mapperly.Abstractions;
using Serilog;

namespace Application.Mapper;

[Mapper]
public partial class ConversationMapper(IStorageService storageService)
{
   public partial ConversationDto ToDto(ConversationEntity source);
   
   [MapProperty(nameof(ConversationParticipant.UserId), nameof(SimpleUserDto.Id))]
   [MapProperty(nameof(ConversationParticipant.UserId), nameof(SimpleUserDto.Avatar), Use = nameof(AvatarUrl))]
   [MapProperty(nameof(ConversationParticipant.User) + "." + nameof(UserEntity.UserName), nameof(SimpleUserDto.Username), Use = nameof(AtSymbol))]
   private partial SimpleUserDto ParticipantToSimpleUserDto(ConversationParticipant source);


   [UserMapping(Default = false)]
   private string AtSymbol(string username) => username.StartsWith("@") ? username : "@" + username;
   
   [UserMapping(Default = false)]
   private string AvatarUrl(Guid userId) => storageService.GetUserAvatar(userId);
}