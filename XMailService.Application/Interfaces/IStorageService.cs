namespace XMailService.Application.Interfaces;

public interface IStorageService
{
    Task UploadAsync(string key, string body, CancellationToken cancellationToken = default);
}
