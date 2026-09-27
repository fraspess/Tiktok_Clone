using Domain.Entities.HashTags;

namespace Domain.Entities.Video;

public class VideoHashTagEntity
{
    public Guid VideoId { get; set; }
    public VideoEntity Video { get; set; } = null!;

    public Guid HashTagId { get; set; }
    public HashTagEntity HashTag { get; set; } = null!;
}