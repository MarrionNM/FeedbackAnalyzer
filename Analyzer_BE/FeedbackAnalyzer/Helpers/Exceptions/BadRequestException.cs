namespace FeedbackAnalyzer.Helpers.Exceptions;

public class BadRequestException : Exception
{
    public IEnumerable<string> Errors { get; } = [];

    public BadRequestException(string message) : base(message) { }

    public BadRequestException(string message, IEnumerable<string> errors)
        : base(message)
    {
        Errors = errors ?? [];
    }
}