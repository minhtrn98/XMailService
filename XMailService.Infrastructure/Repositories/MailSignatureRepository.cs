using XMailService.Application.Interfaces;
using XMailService.Domain.Entities;
using XMailService.Infrastructure.Persistence;

namespace XMailService.Infrastructure.Repositories;

public sealed class MailSignatureRepository(AppDbContext dbContext, ICurrentUserProvider currentUserProvider)
    : Repository<MailSignature>(dbContext, currentUserProvider), IMailSignatureRepository
{
}
