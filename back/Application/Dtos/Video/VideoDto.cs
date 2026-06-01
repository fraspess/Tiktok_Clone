using Application.Dtos.User;

namespace Application.Dtos.Video
{
    public class VideoDto
    {
        public Guid Id { get; set; }
        public string VideoUrl { get; set; }

        public string Description { get; set; } = String.Empty;

        public List<string> HashTags { get; set; } = new List<string>();
        
        public string ThumbnailUrl { get; set; } = String.Empty;

        public int LikeCount { get; set; }

        public int CommentsCount { get; set; }

        public int FavoriteCount { get; set; }

        public bool IsFavorited { get; set; }
        public bool IsLiked { get; set; }

        public UserAuthorDto? Author { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}