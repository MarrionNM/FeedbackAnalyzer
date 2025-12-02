using System.Net;

namespace FeedbackAnalyzer.Helpers.Exceptions;

public abstract class BaseException(string message, IEnumerable<string> errors = null, Exception innerException = null) : Exception(message, innerException)
{
    public abstract HttpStatusCode StatusCode { get; }
    public IEnumerable<string> Errors { get; set; } = errors ?? [];
}