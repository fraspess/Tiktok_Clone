namespace Application.Dtos.User;

public class SimpleUserDto
{
    public Guid Id { get; set; }
    public object Avatar { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}