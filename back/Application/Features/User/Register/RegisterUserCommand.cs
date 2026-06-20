using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.User.Register;

public record RegisterUserCommand(string Username, string Email, string Password, IFormFile? Avatar)
    : IRequest<Unit>;