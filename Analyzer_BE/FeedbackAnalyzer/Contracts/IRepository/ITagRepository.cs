using FeedbackAnalyzer.Data.DTO;

namespace FeedbackAnalyzer.Contracts.IRepository;

public interface ITagRepository
{
    Task<TagDTO> GetOrCreateTagAsync(string tagName);
    Task AttachFeedbackTagAsync(string feedbackId, string tagId);
    Task<List<TagDTO>> GetTags();
}
