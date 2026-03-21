using InterviewPlatform.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPlatform.Application.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        if (exception is NotFoundException notFoundException)
        {
            problemDetails.Title = "Resource not found";
            problemDetails.Status = StatusCodes.Status404NotFound;
            problemDetails.Detail = notFoundException.Message;
        }
        else if (exception is ValidationException validationException)
        {
            problemDetails.Title = "Validation Error";
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Detail = validationException.Message;
        }
        else if (exception is ForbiddenException forbiddenException)
        {
            problemDetails.Title = "Forbidden";
            problemDetails.Status = StatusCodes.Status403Forbidden;
            problemDetails.Detail = forbiddenException.Message;
        }
        else if (exception is InvalidOperationException invalidOpException)
        {
            problemDetails.Title = "Invalid Operation";
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Detail = invalidOpException.Message;
        }
        else
        {
            problemDetails.Title = "An unexpected error occurred.";
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Detail = exception.Message; // In production, consider hiding stack details
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}
