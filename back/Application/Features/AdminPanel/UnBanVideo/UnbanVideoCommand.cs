using MediatR;

namespace Application.Features.AdminPanel.UnBanVideo;

public record UnbanVideoCommand(Guid VideoId) : IRequest<Unit>;