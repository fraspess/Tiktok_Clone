using Application.Interfaces;
using Domain.Entities.HashTags;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.HashTag
{
    internal class HashTagService(IAppDbContext appDbContext) : IHashTagService
    {
        public async Task<List<HashTagEntity>> GetOrCreateAsync(List<string> tags)
        {
            var result = new List<HashTagEntity>();
            foreach (var tagName in tags)
            {
                var tag = await appDbContext.HashTags.Where(t => t.Tag == tagName).FirstOrDefaultAsync()
                          ?? appDbContext.GetTracked<HashTagEntity>(t => t.Tag == tagName);

                if (tag is null)
                {
                    tag = new HashTagEntity { Tag = tagName };
                    await appDbContext.HashTags.AddAsync(tag);
                }

                result.Add(tag);
            }

            await appDbContext.SaveChangesAsync();
            return result;
        }
    }
}