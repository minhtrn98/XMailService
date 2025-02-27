using Microsoft.EntityFrameworkCore;
using XMailService.Application.Interfaces;
using XMailService.Application.Repositories;
using XMailService.Domain.Entities;
using XMailService.Infrastructure.Persistence;

namespace XMailService.Infrastructure.Repositories;

public sealed class MailSignatureRepository(AppDbContext dbContext, ICurrentUserProvider currentUserProvider)
    : Repository<MailSignature>(dbContext, currentUserProvider), IMailSignatureRepository
{
    public async Task<bool> Delete(string name, CancellationToken cancellationToken = default)
        => await DbContext.MailSignatures
            .Where(x => x.Name == name)
            .ExecuteUpdateAsync(x => x.SetProperty(y => y.SoftDeleted, true), cancellationToken) > 0;

    public Task<int> GetCurrentVersion(string name, CancellationToken cancellationToken = default)
        => DbContext.MailSignatures
            .Where(x => x.Name == name)
            .Select(x => x.Version)
            .FirstOrDefaultAsync(cancellationToken);
}
