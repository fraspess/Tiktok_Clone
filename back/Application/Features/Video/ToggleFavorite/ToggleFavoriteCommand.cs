using MediatR;

namespace Application.Features.Video.ToggleFavorite;

public record ToggleFavoriteCommand(string VideoId) : IRequest<Unit>;