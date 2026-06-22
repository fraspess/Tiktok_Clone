using MediatR;

namespace Application.Features.User.LogOutOnAllDevices;

public record LogOutOnAllDevicesCommand() : IRequest<Unit>;