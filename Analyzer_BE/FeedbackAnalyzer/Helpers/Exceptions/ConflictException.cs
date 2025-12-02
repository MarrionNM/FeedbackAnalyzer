using System.Net;

namespace FeedbackAnalyzer.Helpers.Exceptions;

public class ConflictException(string message, IEnumerable<string> errors) : BaseException(message, errors)
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
}