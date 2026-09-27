using MediatR;

namespace Application.Features.AdminPanel.UnbanVideo;

public record UnbanVideoCommand(Guid VideoId) : IRequest<Unit>;