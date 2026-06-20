using MediatR;

namespace Application.Features.Video.ToggleFavorite;

public record ToggleFavoriteCommand(Guid VideoId) : IRequest<Unit>;