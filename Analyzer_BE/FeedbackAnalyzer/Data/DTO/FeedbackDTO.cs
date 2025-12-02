namespace FeedbackAnalyzer.Data.DTO;

public class FeedbackDTO : BaseDTO
{
    public string Text { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }

    public FeedbackAnalysisDTO? Analysis { get; set; }
    public List<string> Tags { get; set; } = [];
}
