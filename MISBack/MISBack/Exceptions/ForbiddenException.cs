namespace BlogApi.Exceptions;

public class ForbiddenException : Exception
{
    public int StatusCode { get; } = 403;

    public ForbiddenException(string message) : base(message)
    {
        StatusCode = StatusCodes.Status403Forbidden;
    }
}