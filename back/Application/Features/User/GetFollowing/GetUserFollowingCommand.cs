using Application.Dtos.User;
using Application.Pagination;
using MediatR;

namespace Application.Features.User.GetFollowing;

public record GetUserFollowingCommand(string Username, PaginationSettings PaginationSettings) : IRequest<PagedResult<SimpleUserDto>>;