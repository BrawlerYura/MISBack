using System.Net;
using System.Text.Json;
using BlogApi.Exceptions;
using MISBack.Data.Models;

namespace MISBack.MiddleWares;

public class GlobalExceptionHandlingMiddleware : IMiddleware
{

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (BadHttpRequestException badHttpRequestException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            ExceptionDetails problem = new()
            {
                Status = (int)HttpStatusCode.BadRequest,
                Type = "Bad Request",
                Title = "Bad Request",
                Detail = badHttpRequestException.Message
            };

            string json = JsonSerializer.Serialize(problem);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }
        catch (ForbiddenException forbiddenException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

            ExceptionDetails problem = new()
            {
                Status = (int)HttpStatusCode.Forbidden,
                Type = "Forbidden",
                Title = "Forbidden",
                Detail = forbiddenException.Message
            };

            string json = JsonSerializer.Serialize(problem);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }
    }
}