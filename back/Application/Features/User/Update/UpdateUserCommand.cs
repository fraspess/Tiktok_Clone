using Application.Dtos.User;
using MediatR;

namespace Application.Features.User.Update;

public record UpdateUserCommand(UpdateUserDTO dto) : IRequest<Unit>;