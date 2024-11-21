using System.Net;
using System.Security.Authentication;
using API.Middlewares.Models;
using Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace API.Middlewares;

public class GlobalExceptionHandlingMiddleware(
    ILogger<GlobalExceptionHandlingMiddleware> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = exception switch
        {
            NotFoundException => (int)HttpStatusCode.NotFound,
            ValidationException or InsufficientPermissionsException => (int)HttpStatusCode.BadRequest,
            AuthenticationException => (int)HttpStatusCode.Unauthorized,
            UserBannedException => (int)HttpStatusCode.Forbidden,
            TaskCanceledException or OperationCanceledException => default,
            _ => (int)HttpStatusCode.InternalServerError
        };

        if (httpContext.Response.StatusCode == (int)HttpStatusCode.InternalServerError)
        {
            logger.LogCritical(exception, exception.Message);
        }


        if (exception is TaskCanceledException or OperationCanceledException)
        {
            return true;
        }

        await CreateExceptionResponseAsync(httpContext, exception);
        return true;
    }


    private static Task CreateExceptionResponseAsync(HttpContext context, Exception e)
    {
        context.Response.ContentType = "application/json";

        return context.Response.WriteAsync(
            new ErrorDetails(context.Response.StatusCode, e.Message, e.StackTrace ?? "").ToString()
        );
    }
}
