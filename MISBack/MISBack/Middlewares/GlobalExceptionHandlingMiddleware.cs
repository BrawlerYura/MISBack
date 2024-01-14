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
        catch (KeyNotFoundException keyNotFoundException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;

            ExceptionDetails problem = new()
            {
                Status = (int)HttpStatusCode.NotFound,
                Type = "Not Found",
                Title = "Not Found",
                Detail = keyNotFoundException.Message
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
        catch (ConflictException conflictException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;

            ExceptionDetails problem = new()
            {
                Status = (int)HttpStatusCode.Conflict,
                Type = "Conflict",
                Title = "Conflict",
                Detail = conflictException.Message
            };

            string json = JsonSerializer.Serialize(problem);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }
        catch (UnauthorizedAccessException unauthorizedAccessException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            ExceptionDetails problem = new()
            {
                Status = (int)HttpStatusCode.Unauthorized,
                Type = "Unauthorized",
                Title = "Unauthorized",
                Detail = unauthorizedAccessException.Message
            };

            string json = JsonSerializer.Serialize(problem);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }
        catch (Exception e)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            ExceptionDetails problem = new()
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Type = "Server error",
                Title = "Server error",
                Detail = e.Message
            };

            string json = JsonSerializer.Serialize(problem);

            context.Response.ContentType = "application/json";
            
            await context.Response.WriteAsync(json);
        }
    }
}