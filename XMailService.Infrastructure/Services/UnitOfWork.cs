using XMailService.Application.Interfaces;
using XMailService.Infrastructure.Interceptors;
using XMailService.Infrastructure.Persistence;
using XMailService.Infrastructure.Repositories;

namespace XMailService.Infrastructure.Services;

public sealed partial class UnitOfWork(
    AppDbContext context,
    ICurrentUserProvider currentUserProvider,
    IEnumerable<IUnitOfWorkInterceptor> interceptors
    ) : IUnitOfWork
{
    public AppDbContext Context { get; } = context;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        List<IUnitOfWorkInterceptor> orderedInterceptors =
            [.. interceptors.OrderByDescending(x => x.Priority)];

        foreach (IUnitOfWorkInterceptor interceptor in orderedInterceptors)
        {
            await interceptor.BeforeSaveChanges(Context);
        }

        int res = await Context.SaveChangesAsync(cancellationToken);

        foreach (IUnitOfWorkInterceptor interceptor in orderedInterceptors)
        {
            await interceptor.AfterSaveChanges(Context);
        }

        return res;
    }

    public async ValueTask DisposeAsync() => await Context.DisposeAsync();

    public void Dispose() => Context.Dispose();
}

public sealed partial class UnitOfWork
{
    private IMailTemplateRepository? _mailTemplates;
    private IMailSignatureRepository? _mailSignatures;

    public IMailTemplateRepository MailTemplates =>
        _mailTemplates ??= new MailTemplateRepository(Context, currentUserProvider);
    public IMailSignatureRepository MailSignatures =>
        _mailSignatures ??= new MailSignatureRepository(Context, currentUserProvider);
}
