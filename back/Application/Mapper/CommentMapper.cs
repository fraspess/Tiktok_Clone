using Application.Dtos.Comment;
using Riok.Mapperly.Abstractions;

namespace Application.Mapper;

[Mapper]
public partial class CommentMapper
{
    public partial CommentDto ToDto(CommentProjectionDto source);
}