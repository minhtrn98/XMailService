namespace XMailService.Shared.Result;

public sealed class Error
{
    public Exception? Exception { get; init; }

    public string Code { get; }

    public string Description { get; }

    public string? PropertyName { get; }

    public ErrorType Type { get; }

    public Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    public Error(string code, string propertyName, string description)
    {
        Code = code;
        Description = description;
        PropertyName = propertyName;
        Type = ErrorType.Validation;
    }

    public static Error Failure(string code, string description, Exception? ex = null)
    {
        return new Error(code, description, ErrorType.Failure)
        {
            Exception = ex
        };
    }

    public static Error NotFound(string code, string description, Exception? ex = null)
    {
        return new Error(code, description, ErrorType.NotFound)
        {
            Exception = ex
        };
    }

    public static Error Conflict(string code, string description, Exception? ex = null)
    {
        return new Error(code, description, ErrorType.Conflict)
        {
            Exception = ex
        };
    }

    public static Error Forbidden(string code, string description, Exception? ex = null)
    {
        return new Error(code, description, ErrorType.Forbidden)
        {
            Exception = ex
        };
    }

    public static Error Unauthorized(string code, string description, Exception? ex = null)
    {
        return new Error(code, description, ErrorType.Unauthorized)
        {
            Exception = ex
        };
    }

    public static Error Validation(string code, string propertyName, string description)
    {
        return new Error(code, propertyName, description);
    }
}
