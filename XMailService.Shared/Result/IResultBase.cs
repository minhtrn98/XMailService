namespace XMailService.Shared.Result;

public interface IResultBase
{
    IReadOnlyList<Error> Errors { get; }

    bool IsError { get; }

    bool IsSuccess { get; }
}
