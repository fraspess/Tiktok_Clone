using Domain;
using MediatR;

namespace Application.Features.AdminPanel.BanUser;

public record BanUserCommand(Guid Id, UserReportReasons Reason) : IRequest<Unit>;