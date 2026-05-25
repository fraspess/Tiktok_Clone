using Application.Dtos.User;
using MediatR;

namespace Application.Features.AdminPanel.GetUserById;

public record GetUserByIdCommand(Guid Id) : IRequest<GetUserAdminDTO>;