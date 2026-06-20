using Microsoft.AspNetCore.Http;

namespace Application.Dtos.User;

public class RegisterUserDto
{
    public required string Username { get; set; }
    public required string Email { get; set; }

    public required string Password { get; set; }

    public IFormFile? Avatar { get; set; }
}