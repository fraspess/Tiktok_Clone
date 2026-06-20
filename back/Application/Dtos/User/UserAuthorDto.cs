namespace Application.Dtos.User;

public class UserAuthorDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public object Avatar { get; set; } = string.Empty;
}