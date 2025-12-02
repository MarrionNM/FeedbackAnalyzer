namespace FeedbackAnalyzer.Data.Response;

public class ErrorResponse(string message)
{
    public string Message { get; init; } = message;
}
