using InterviewPlatform.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPlatform.Application.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = StatusCodes.Status500InternalServerError;
        var message = "An unexpected error occurred.";

        if (exception is NotFoundException notFoundException)
        {
            statusCode = StatusCodes.Status404NotFound;
            message = notFoundException.Message;
        }
        else if (exception is ValidationException validationException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            message = validationException.Message;
        }
        else if (exception is ForbiddenException forbiddenException)
        {
            statusCode = StatusCodes.Status403Forbidden;
            message = forbiddenException.Message;
        }
        else if (exception is InvalidOperationException invalidOpException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            message = invalidOpException.Message;
        }
        else
        {
            message = exception.Message; // Consider hiding this in production
        }

        var response = InterviewPlatform.Application.DTOs.ApiResponse<object>.Fail(message, statusCode);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        
        return true;
    }
}
