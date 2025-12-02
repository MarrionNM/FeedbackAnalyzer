namespace FeedbackAnalyzer.Data.Models;

public class FeedbackAnalysis : Base
{
    public required string FeedbackId { get; set; }
    public Feedback Feedback { get; set; } = default!;

    public string Summary { get; set; } = string.Empty;
    public string Sentiment { get; set; } = string.Empty;
    public string Priority { get; set; } = "P3";
    public string NextAction { get; set; } = string.Empty;
}
