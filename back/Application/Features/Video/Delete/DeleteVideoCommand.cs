using MediatR;

namespace Application.Features.Video.Delete;

public record DeleteVideoCommand(Guid VideoId) : IRequest<Unit>;