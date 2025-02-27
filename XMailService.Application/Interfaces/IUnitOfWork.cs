using XMailService.Application.Repositories;

namespace XMailService.Application.Interfaces;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IMailTemplateRepository MailTemplates { get; }
    IMailSignatureRepository MailSignatures { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
