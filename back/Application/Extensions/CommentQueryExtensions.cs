using Application.Dtos.Comment;
using Domain.Entities.Comment;
using Microsoft.Extensions.Configuration;

namespace Application.Extensions;

public static class CommentQueryExtensions
{
    // include author
    public static IQueryable<CommentProjectionDto> ToProjectionDto(this IQueryable<CommentEntity> query, Guid? currentUserId)
    {
        return query.Select(c => new CommentProjectionDto()
        {
            Id = c.Id,
            Text = c.Text,
            RepliesCount = c.Replies.Count,
            OwnerUsername = "@" + c.Author.UserName,
            IsLiked = c.CommentLikes.Any(l => l.UserId == currentUserId),
            LikesCount = c.CommentLikes.Count,
            CreatedAt = c.CreatedAt,
        });
    }
}