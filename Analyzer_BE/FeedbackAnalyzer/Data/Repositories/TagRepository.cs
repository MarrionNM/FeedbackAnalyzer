using AutoMapper;
using FeedbackAnalyzer.Contracts.IRepository;
using FeedbackAnalyzer.Data.DTO;
using FeedbackAnalyzer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FeedbackAnalyzer.Data.Repositories;

public class TagRepository(ApplicationDbContext context, IMapper mapper) : ITagRepository
{
    public async Task<TagDTO> GetOrCreateTagAsync(string tagName)
    {
        var existing = await context.Tags
            .FirstOrDefaultAsync(tag => tag.Name.ToLower() == tagName.ToLower());

        if (existing != null)
            return mapper.Map<TagDTO>(existing);

        var newTag = new TagDTO { Name = tagName };
        context.Tags.Add(mapper.Map<Tag>(newTag));
        await context.SaveChangesAsync();

        return newTag;
    }

    public async Task AttachFeedbackTagAsync(string feedbackId, string tagId)
    {
        var ft = new FeedbackTag
        {
            FeedbackId = feedbackId,
            TagId = tagId
        };

        context.FeedbackTags.Add(ft);
        await context.SaveChangesAsync();
    }

    public async Task<List<TagDTO>> GetTags()
    {
        return mapper.Map<List<TagDTO>>(await context.Tags.ToListAsync());
    }
}
