using FeedbackAnalyzer.Data.DTO;
using FeedbackAnalyzer.Data.Payloads;
using FeedbackAnalyzer.Data.Response;

namespace FeedbackAnalyzer.Contracts.IRepository;

public interface IFeedbackRepository
{
    Task<FeedbackDTO> CreateFeedback(FeedbackDTO feedbackPayload, float[] vectorEmbedding);
    Task<FeedbackDTO?> GetFeedbackById(string id);
    Task<PagedResponse<FeedbackDTO>> GetFeedbacks(Filter filter);
}
