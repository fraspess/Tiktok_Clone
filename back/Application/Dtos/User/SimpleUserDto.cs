namespace Application.Dtos.User
{
    public class SimpleUserDto
    {
        public Guid Id { get; set; }
        public string Avatar { get; set; } = String.Empty;
        public string Username { get; set; } = String.Empty;
    }
}