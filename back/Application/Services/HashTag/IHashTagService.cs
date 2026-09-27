using Domain.Entities.HashTags;

namespace Application.Services.HashTag;

public interface IHashTagService
{
    public Task<List<HashTagEntity>> GetOrCreateAsync(List<string> tags);
}