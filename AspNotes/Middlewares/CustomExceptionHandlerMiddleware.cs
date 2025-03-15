using Microsoft.AspNetCore.Mvc;

namespace AspNotes.Middlewares;

/// <summary>
/// Middleware to handle exceptions and return a standardized error response.
/// </summary>
public class CustomExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<CustomExceptionHandlerMiddleware> logger,
    IHostEnvironment env)
{
    /// <summary>
    /// Invokes the middleware to handle exceptions.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task that represents the completion of request processing.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred: {ErrorMessage}", ex.Message);

            var message = env.IsDevelopment() ? ex.Message : "Server error";

            var problemDetails = new ProblemDetails
            {
                Title = message,
                Status = StatusCodes.Status500InternalServerError,
                Detail = env.IsDevelopment() ? ex.StackTrace : null,
                Instance = context.Request.Path,
            };

            context.Response.StatusCode = problemDetails.Status.Value;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}