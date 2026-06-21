using Application.Dtos.User;

namespace Application.Dtos.Video;

public class SimpleVideoDto
{
    public string Id { get; set; }
    public string VideoUrl { get; set; }

    public string Description { get; set; } = string.Empty;

    public List<string> HashTags { get; set; } = new();
    public UserAuthorDto? Author { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ViewCoun { get; set; }
    public string ThumbnailUrl { get; set; } = string.Empty;
}