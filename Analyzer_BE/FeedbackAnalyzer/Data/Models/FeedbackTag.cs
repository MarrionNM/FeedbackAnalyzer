namespace FeedbackAnalyzer.Data.Models;

public class FeedbackTag
{
    public required string FeedbackId { get; set; }
    public Feedback Feedback { get; set; } = default!;
    public required string TagId { get; set; }
    public Tag Tag { get; set; } = default!;
}
