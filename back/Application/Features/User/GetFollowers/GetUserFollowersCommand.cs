using Application.Dtos.User;
using Application.Pagination;
using MediatR;

namespace Application.Features.User.GetFollowers;

public record GetUserFollowersCommand(string Username, PaginationSettings PaginationSettings)
    : IRequest<PagedResult<SimpleUserDto>>;