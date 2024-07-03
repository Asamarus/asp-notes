using Microsoft.AspNetCore.Mvc;

namespace AspNotes.Web.Middlewares;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger, IHostEnvironment env)
{
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
                Instance = context.Request.Path
            };

            context.Response.StatusCode = problemDetails.Status.Value;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
