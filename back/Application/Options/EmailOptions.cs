namespace Application.Options
{
    public class EmailOptions
    {
        public string Host { get; set; } = String.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string FromName { get; set; } = String.Empty;
    }
}