using Application.Dtos.User;
using MediatR;

namespace Application.Features.User.GetCurrentUser;

public record GetCurrentUserQuery() : IRequest<UserMeDto>;