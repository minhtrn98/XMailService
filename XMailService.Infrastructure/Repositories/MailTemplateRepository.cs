using XMailService.Application.Interfaces;
using XMailService.Domain.Entities;
using XMailService.Infrastructure.Persistence;

namespace XMailService.Infrastructure.Repositories;

public sealed class MailTemplateRepository(AppDbContext dbContext, ICurrentUserProvider currentUserProvider)
    : Repository<MailTemplate>(dbContext, currentUserProvider), IMailTemplateRepository
{
}

