using Amazon.S3;
using Amazon.S3.Model;
using XMailService.Application.Interfaces;

namespace XMailService.Infrastructure.Services;

public sealed class StorageService(IAmazonS3 s3Client) : IStorageService
{
    private readonly string _bucketName = "xmail-service";

    public async Task UploadAsync(string key, string body, CancellationToken cancellationToken = default)
    {
        PutObjectRequest request = new()
        {
            BucketName = _bucketName,
            Key = key,
            ContentBody = body
        };

        await s3Client.PutObjectAsync(request, cancellationToken);
    }
}
