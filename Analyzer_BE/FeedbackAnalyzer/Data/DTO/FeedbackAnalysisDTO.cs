namespace FeedbackAnalyzer.Data.DTO;

public class FeedbackAnalysisDTO : BaseDTO
{
    public string FeedbackId { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string Sentiment { get; set; } = string.Empty;
    public string Priority { get; set; } = "Medium";
    public string NextAction { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
}
