using Application.Dtos.User;
using Application.Dtos.Video;
using Domain.Entities.Video;
using Microsoft.EntityFrameworkCore;

namespace Application.Extensions;

public static class VideoQueryExtensions
{
    public static IQueryable<VideoProjectionDto> ToProjectionDto(this IQueryable<VideoEntity> query,
        Guid? currentUserId)
    {
        return query.Select(v => new VideoProjectionDto
        {
            Id = v.Id,
            Description = v.Description,
            HashTags = v.HashTags.Select(h => h.HashTag.Tag).ToList(),
            LikeCount = v.LikeCount,
            CommentsCount = v.CommentCount,
            FavoriteCount = v.FavoriteCount,
            Status = v.Status,
            ProccessedInPercents = v.ProccessedInPercents,
            Author = new UserAuthorDto
            {
                Id = v.Author.Id,
                Username = v.Author.UserName
            },
            IsFavorited = v.Favorites.Any(f => f.UserId == currentUserId),
            IsLiked = v.Likes.Any(l => l.UserId == currentUserId),
            CreatedAt = v.CreatedAt
        });
    }
}