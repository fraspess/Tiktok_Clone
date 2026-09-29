using Application.Dtos.Comment;
using Application.Dtos.User;
using Application.Interfaces;
using Riok.Mapperly.Abstractions;

namespace Application.Mapper;

[Mapper]
public partial class CommentMapper(IStorageService storageService)
{
    [MapProperty(nameof(CommentProjectionDto.AuthorId), nameof(CommentDto.AvatarUrl), Use = nameof(GetUserAvatar))]
    public partial CommentDto ToDto(CommentProjectionDto source);
    
    private AvatarDto GetUserAvatar(Guid id)
    {
        return storageService.GetUserAvatar(id);
    }
}
