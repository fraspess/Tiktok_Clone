namespace Application.Dtos.User;

public class SeedUserDto
{
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? Image { get; set; }

    public string? Password { get; set; }

    public string[]? Roles { get; set; }
}