namespace FeedbackAnalyzer.Data.Response;

public class Response<T>
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = "";
    public T? Data { get; init; }
}
