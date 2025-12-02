namespace FeedbackAnalyzer.Data.Payloads;

public class FeedbackPayload
{
    public required string Message { get; set; }
    public string? Email { get; set; }
}
