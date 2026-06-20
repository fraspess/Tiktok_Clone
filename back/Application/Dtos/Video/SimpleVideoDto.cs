using Application.Dtos.User;

namespace Application.Dtos.Video;

public class SimpleVideoDto
{
    public Guid Id { get; set; }
    public string VideoUrl { get; set; }

    public string Description { get; set; } = string.Empty;

    public List<string> HashTags { get; set; } = new();
    public UserAuthorDto? Author { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ThumbnailUrl { get; set; } = string.Empty;
}