namespace XMailService.Shared.Result;

public readonly record struct Success;

public static class Results
{
    private static readonly Success _successValue = new();
    public static Result<Success> Success => new(_successValue);
}
