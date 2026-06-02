namespace Application.Dtos.User
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } = String.Empty;

        public string NewPassword { get; set; } = String.Empty;

        public string Token { get; set; } = String.Empty;
    }
}