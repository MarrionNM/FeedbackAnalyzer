using FeedbackAnalyzer.Data.DTO;

namespace FeedbackAnalyzer.Contracts.IServices;

public interface IAnalysisService
{
    Task<FeedbackAnalysisDTO> AnalyzeAsync(string feedback);
    Task<float[]> GenerateEmbeddingAsync(string text);
}
