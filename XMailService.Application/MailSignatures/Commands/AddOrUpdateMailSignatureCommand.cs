using System.Text;
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
            version++;

            string fileName = $"{request.Name}_V{version}.html";
            string filePath = Path.Combine(Path.GetTempPath(), fileName);
            MailSignature mailSignature = new()
            {
                Name = request.Name,
                Description = request.Description,
                Version = version
            };
            bool isSaveDbSuccess = false;

            try
            {
                unitOfWork.MailSignatures.Add(mailSignature);
                await unitOfWork.SaveChangesAsync();
                isSaveDbSuccess = true;

                using FileStream fs = new(filePath, FileMode.Create);
                byte[] byteBody = Encoding.UTF8.GetBytes(request.Body);
                await fs.WriteAsync(byteBody);

                PutObjectRequest putRequest = new()
                {
                    BucketName = _bucketName,
                    Key = "signatures",
                    InputStream = fs,
                    ContentType = "text/html",
                    Metadata =
                    {
                        ["x-amz-meta-origionalname"] = fileName,
                        ["x-amz-meta-extension"] = Path.GetExtension(fileName)
                    }
                };

                await s3Client.PutObjectAsync(putRequest);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while creating mail template");
                if (isSaveDbSuccess)
                {
                    unitOfWork.MailSignatures.Delete(mailSignature);
                    await unitOfWork.SaveChangesAsync();
                }
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
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
