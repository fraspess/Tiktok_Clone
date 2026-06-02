using Microsoft.AspNetCore.Http;

namespace Application.Dtos.Video
{
    public class CreateVideoDto
    {
        public required IFormFile VideoFile { get; set; }

        public required string Description { get; set; }
    }
}