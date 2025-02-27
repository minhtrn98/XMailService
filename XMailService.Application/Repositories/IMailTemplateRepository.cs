using XMailService.Domain.Entities;

namespace XMailService.Application.Repositories;

public interface IMailTemplateRepository : IRepository<MailTemplate>
{
    Task<int> GetCurrentVersion(string name, CancellationToken cancellationToken = default);

    Task<bool> Delete(string name, CancellationToken cancellationToken = default);
}
