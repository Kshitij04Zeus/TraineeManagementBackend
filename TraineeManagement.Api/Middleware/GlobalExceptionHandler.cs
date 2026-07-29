using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.CustomExceptions;
 
namespace TraineeManagement.Api.Middleware;
 
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
 
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
  
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Error occurred: {Message}", exception.Message);
 
        var (statusCode, title, detail) = exception switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found", exception.Message),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid Input", exception.Message),
            PayloadTooLargeException => (StatusCodes.Status413PayloadTooLarge, "Payload Too Large", exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred. Please try again later.")
        };
 
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };
 
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
 
        return true;
    }
}
 