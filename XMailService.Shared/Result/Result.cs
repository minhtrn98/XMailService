// Ignore Spelling: Deconstruct

namespace XMailService.Shared.Result;

public sealed class Result<TValue> : ResultBase
{
    private readonly TValue? _value;
    public TValue Value => _value!;

    public Result(TValue value) : base([]) => _value = value;

    public Result(Error error)
        : base(error)
    {
    }

    public Result(List<Error> errors)
        : base(errors)
    {
    }

    public static Result<TValue> NullValue => new(default(TValue)!);

    public TMatchResult Match<TMatchResult>(Func<TValue, TMatchResult> onValue, Func<IReadOnlyList<Error>, TMatchResult> onError) => IsError ? onError(Errors) : onValue(Value);

    public static implicit operator Result<TValue>(TValue value) => new(value);

    public static implicit operator Result<TValue>(Error error) => new(error);

    public static implicit operator Result<TValue>(List<Error> errors) => new(errors);

    public void Deconstruct(out TValue value, out IReadOnlyList<Error> errors)
    {
        value = Value;
        errors = Errors;
    }

    public void Deconstruct(out TValue value, out IReadOnlyList<Error> errors, out bool isError)
    {
        value = Value;
        errors = Errors;
        isError = IsError;
    }
}
