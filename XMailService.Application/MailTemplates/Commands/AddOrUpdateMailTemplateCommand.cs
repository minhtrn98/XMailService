using Amazon.S3;
using Amazon.S3.Model;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using XMailService.Application.Interfaces;
using XMailService.Domain.Entities;

namespace XMailService.Application.MailTemplates.Commands;

public sealed record AddOrUpdateMailTemplateCommand : IRequest
{
    public required string Name { get; init; }
    public required string Subject { get; init; }
    public required string Body { get; init; }
    public string? Description { get; set; }

    internal sealed class Handler(
        IUnitOfWork unitOfWork,
        IAmazonS3 s3Client,
        ILogger<AddOrUpdateMailTemplateCommand> logger
        ) : IRequestHandler<AddOrUpdateMailTemplateCommand>
    {
        private readonly string _bucketName = "xmailservicebucket";

        public async Task Handle(AddOrUpdateMailTemplateCommand request, CancellationToken cancellationToken)
        {
            int version = await unitOfWork.MailTemplates.GetCurrentVersion(request.Name, cancellationToken);
            int nextVersion = version + 1;

            string fileName = $"{request.Name}_V{nextVersion}.txt";
            bool isSaveDbSuccess = false;

            MailTemplate mailTemplate = CreateMailTemplate(request, nextVersion);

            try
            {
                isSaveDbSuccess = await SaveMailTemplateToDatabase(mailTemplate);
                await UploadFileToS3(request.Body, fileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while creating mail template");
                if (isSaveDbSuccess)
                {
                    await RollbackDatabaseChanges(mailTemplate);
                }
            }
        }

        private static MailTemplate CreateMailTemplate(AddOrUpdateMailTemplateCommand request, int version) => new()
        {
            Name = request.Name,
            Subject = request.Subject,
            Description = request.Description,
            Version = version
        };

        private async Task<bool> SaveMailTemplateToDatabase(MailTemplate mailTemplate)
        {
            unitOfWork.MailTemplates.Add(mailTemplate);
            return await unitOfWork.SaveChangesAsync() > 0;
        }

        private async Task UploadFileToS3(string body, string fileName)
        {
            PutObjectRequest putRequest = new()
            {
                BucketName = _bucketName,
                Key = $"templates/{fileName}",
                ContentBody = body,
            };

            await s3Client.PutObjectAsync(putRequest);
        }

        private async Task RollbackDatabaseChanges(MailTemplate mailSignature)
        {
            unitOfWork.MailTemplates.Delete(mailSignature);
            await unitOfWork.SaveChangesAsync();
        }
    }

    public sealed class Validator : AbstractValidator<AddOrUpdateMailTemplateCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Subject).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Body).NotEmpty();
        }
    }
}
