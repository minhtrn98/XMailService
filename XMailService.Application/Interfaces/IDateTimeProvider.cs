namespace XMailService.Application.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
