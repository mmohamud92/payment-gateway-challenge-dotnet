using System.Net;
using System.Text.Json;

using PaymentGateway.Domain.Exceptions;

namespace PaymentGateway.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        string message = "An unexpected error occurred.";

        if (exception is ArgumentException or InvalidCardNumberException or InvalidExpiryDateException
            or InvalidCvvException)
        {
            statusCode = HttpStatusCode.BadRequest;
            message = exception.Message;
        }

        string result = JsonSerializer.Serialize(new { error = message });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(result);
    }
}