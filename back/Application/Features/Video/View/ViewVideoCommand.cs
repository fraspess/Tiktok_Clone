using MediatR;

namespace Application.Features.Video.View;

public record ViewVideoCommand(string Id) : IRequest<Unit>;