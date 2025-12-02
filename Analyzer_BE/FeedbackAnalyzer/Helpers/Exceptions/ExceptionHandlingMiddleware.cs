using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace FeedbackAnalyzer.Helpers.Exceptions;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        var requestId = context.TraceIdentifier;

        try
        {
            context.Response.Headers["X-Request-ID"] = requestId;
            await _next(context);
        }
        catch (Exception ex)
        {
            sw.Stop();
            await HandleExceptionAsync(context, ex, sw.ElapsedMilliseconds, requestId);
        }
    }

    private Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        long elapsedMs,
        string requestId)
    {
        HttpStatusCode status;
        string message;
        object? errors = null;

        switch (exception)
        {
            case BadRequestException badRequest:
                status = HttpStatusCode.BadRequest;
                message = badRequest.Message;
                errors = badRequest.Errors;
                break;

            case UnauthorizedException unauthorized:
                status = HttpStatusCode.Unauthorized;
                message = unauthorized.Message;
                break;

            case InternalServerException internalEx:
                status = HttpStatusCode.InternalServerError;
                message = internalEx.Message;
                break;

            case ConflictException conflictEx:
                status = HttpStatusCode.Conflict;
                message = conflictEx.Message;
                errors = conflictEx.Errors;
                break;

            default:
                status = HttpStatusCode.InternalServerError;
                message = "An unexpected error occurred.";
                break;
        }

        _logger.LogError(
            exception,
            "Request {method} {path} failed with {statusCode} in {elapsed}ms | RequestId={requestId}",
            context.Request.Method,
            context.Request.Path,
            (int)status,
            elapsedMs,
            requestId
        );

        var response = new
        {
            success = false,
            message,
            requestId,
            errors
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response, _jsonOptions)
        );
    }
}
