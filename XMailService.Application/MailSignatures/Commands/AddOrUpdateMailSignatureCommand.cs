using Amazon.S3;
using Amazon.S3.Model;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using XMailService.Application.Interfaces;
using XMailService.Domain.Entities;

namespace XMailService.Application.MailSignatures.Commands;

public sealed record AddOrUpdateMailSignatureCommand : IRequest
{
    public required string Name { get; init; }
    public required string Body { get; init; }
    public string? Description { get; set; }

    internal sealed class Handler(
        IUnitOfWork unitOfWork,
        IAmazonS3 s3Client,
        ILogger<AddOrUpdateMailSignatureCommand> logger
        ) : IRequestHandler<AddOrUpdateMailSignatureCommand>
    {
        private readonly string _bucketName = "xmailservicebucket";

        public async Task Handle(AddOrUpdateMailSignatureCommand request, CancellationToken cancellationToken)
        {
            int version = await unitOfWork.MailSignatures.GetCurrentVersion(request.Name, cancellationToken);
            int nextVersion = version + 1;

            string fileName = $"{request.Name}_V{nextVersion}.html";
            bool isSaveDbSuccess = false;

            MailSignature mailSignature = CreateMailSignature(request, nextVersion);

            try
            {
                isSaveDbSuccess = await SaveMailSignatureToDatabase(mailSignature);
                await UploadFileToS3(request.Body, fileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while creating mail signature");
                if (isSaveDbSuccess)
                {
                    await RollbackDatabaseChanges(mailSignature);
                }
            }
        }

        private static MailSignature CreateMailSignature(AddOrUpdateMailSignatureCommand request, int version) => new()
        {
            Name = request.Name,
            Description = request.Description,
            Version = version
        };

        private async Task<bool> SaveMailSignatureToDatabase(MailSignature mailSignature)
        {
            unitOfWork.MailSignatures.Add(mailSignature);
            return await unitOfWork.SaveChangesAsync() > 0;
        }

        private async Task UploadFileToS3(string body, string fileName)
        {
            PutObjectRequest putRequest = new()
            {
                BucketName = _bucketName,
                Key = $"signatures/{fileName}",
                ContentBody = body,
            };

            await s3Client.PutObjectAsync(putRequest);
        }

        private async Task RollbackDatabaseChanges(MailSignature mailSignature)
        {
            unitOfWork.MailSignatures.Delete(mailSignature);
            await unitOfWork.SaveChangesAsync();
        }
    }

    public sealed class Validator : AbstractValidator<AddOrUpdateMailSignatureCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Body).NotEmpty();
        }
    }
}
