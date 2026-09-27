using System.ComponentModel.DataAnnotations;
using Domain.Entities.Video;

namespace Domain.Entities.HashTags;

public class HashTagEntity : AuditableEntity
{
    [MaxLength(50)] public required string Tag { get; set; }
    public ICollection<VideoHashTagEntity> VideoHashTags { get; set; } = new List<VideoHashTagEntity>();
}