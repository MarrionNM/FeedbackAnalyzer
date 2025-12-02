namespace FeedbackAnalyzer.Helpers.Exceptions;

public class InternalServerException(string message, Exception inner = null) : Exception(message, inner)
{
}
