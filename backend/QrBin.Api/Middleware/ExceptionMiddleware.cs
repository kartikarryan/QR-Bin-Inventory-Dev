using System.Text.Json;
using QrBin.Api.Exceptions;
using QrBin.ViewModels;

namespace QrBin.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = exception switch
        {
            AppValidationException ex => (400, ex.Message, ex.Errors),
            UnauthorizedException ex => (401, ex.Message, ex.Errors),
            ForbiddenException ex => (403, ex.Message, ex.Errors),
            NotFoundException ex => (404, ex.Message, ex.Errors),
            ConflictException ex => (409, ex.Message, ex.Errors),
            AppException ex => (ex.StatusCode, ex.Message, ex.Errors),
            TaskCanceledException => (499, "Request was cancelled", new List<string>()),
            OperationCanceledException => (499, "Request was cancelled", new List<string>()),
            _ => (500, "Something went wrong. Please try again later.", new List<string>())
        };

        _logger.LogError(exception, "Exception caught by middleware: {StatusCode} - {ExceptionMessage}", statusCode, exception.Message);

        var response = new ApiResponse<object>
        {
            StatusCode = statusCode,
            Message = message,
            Errors = errors.Count > 0 ? errors : null
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
