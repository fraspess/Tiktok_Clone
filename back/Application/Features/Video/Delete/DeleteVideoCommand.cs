using MediatR;

namespace Application.Features.Video.Delete;

public record DeleteVideoCommand(string VideoId) : IRequest<Unit>;