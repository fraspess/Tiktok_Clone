using Application.Dtos.User;
using Application.Interfaces;
using MediatR;

namespace Application.Features.User.GetByUsername
{
    public class GetUserByUsernameQueryHandler(IUserService service, ICurrentUser user) : IRequestHandler<GetUserByUsernameQuery, UserDto>
    {
        public async Task<UserDto> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            return await service.GetByUsernameAsync(request.Username);
        }
    }
}