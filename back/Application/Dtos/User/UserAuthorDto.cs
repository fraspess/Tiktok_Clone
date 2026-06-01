namespace Application.Dtos.User
{
    public class UserAuthorDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }
}