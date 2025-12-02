namespace FeedbackAnalyzer.Helpers.Exceptions;

public class UnauthorizedException(string message) : Exception(message)
{
}