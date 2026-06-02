using MediatR;

namespace Application.Features.AdminPanel.UnbanUser;

public record UnbanUserCommand(Guid Id) : IRequest<Unit>;