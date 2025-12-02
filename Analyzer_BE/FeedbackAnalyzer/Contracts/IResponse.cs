namespace FeedbackAnalyzer.Contracts;

public interface IResponse
{
    public string Message { get; set; }
    public bool IsSuccess { get; set; }
}
