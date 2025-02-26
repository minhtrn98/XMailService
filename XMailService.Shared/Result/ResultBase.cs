namespace XMailService.Shared.Result;

public abstract class ResultBase : IResultBase
{
    private readonly IReadOnlyList<Error>? _errors;

    public bool IsError => Errors.Count > 0;
    public bool IsSuccess => Errors.Count == 0;
    public IReadOnlyList<Error> Errors
    {
        get => _errors ?? [];
        init => _errors = value;
    }

    protected ResultBase(Error error) => _errors = [error];

    protected ResultBase(List<Error> errors) => _errors = errors;
}
