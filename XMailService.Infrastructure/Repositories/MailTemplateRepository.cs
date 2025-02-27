using Microsoft.EntityFrameworkCore;
using XMailService.Application.Interfaces;
using XMailService.Domain.Entities;
using XMailService.Infrastructure.Persistence;

namespace XMailService.Infrastructure.Repositories;

public sealed class MailTemplateRepository(AppDbContext dbContext, ICurrentUserProvider currentUserProvider)
    : Repository<MailTemplate>(dbContext, currentUserProvider), IMailTemplateRepository
{
    public async Task<bool> Delete(string name, CancellationToken cancellationToken = default)
        => await DbContext.MailTemplates
            .Where(x => x.Name == name)
            .ExecuteUpdateAsync(x => x.SetProperty(y => y.SoftDeleted, true), cancellationToken) > 0;

    public Task<int> GetCurrentVersion(string name, CancellationToken cancellationToken = default)
        => DbContext.MailTemplates
            .Where(x => x.Name == name)
            .Select(x => x.Version)
            .FirstOrDefaultAsync(cancellationToken);
}

