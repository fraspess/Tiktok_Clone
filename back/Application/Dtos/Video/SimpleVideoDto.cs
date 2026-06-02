using Application.Dtos.User;

namespace Application.Dtos.Video
{
    public class SimpleVideoDto
    {
        public Guid Id { get; set; }
        public string VideoUrl { get; set; }

        public string Description { get; set; } = String.Empty;

        public List<string> HashTags { get; set; } = new List<string>();
        public UserAuthorDto? Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ThumbnailUrl { get; set; } = String.Empty;
    }
}