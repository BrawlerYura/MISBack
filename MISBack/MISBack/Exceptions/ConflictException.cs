namespace BlogApi.Exceptions;

public class ConflictException : Exception
{
    public int StatusCode { get; } = 409;

    public ConflictException(string message) : base(message)
    {
        StatusCode = StatusCodes.Status409Conflict;
    }
}