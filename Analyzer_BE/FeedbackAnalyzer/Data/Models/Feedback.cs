namespace FeedbackAnalyzer.Data.Models;

public class Feedback : Base
{
    public string Text { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }

    public float[]? VectorEmbedding { get; set; }

    public FeedbackAnalysis? Analysis { get; set; }
    public List<FeedbackTag> FeedbackTags { get; set; } = [];
}
