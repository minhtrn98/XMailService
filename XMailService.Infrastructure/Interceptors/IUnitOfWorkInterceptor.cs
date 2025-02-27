using XMailService.Infrastructure.Persistence;

namespace XMailService.Infrastructure.Interceptors;

public interface IUnitOfWorkInterceptor
{
    int Priority { get; }

    Task BeforeSaveChanges(AppDbContext context);
    Task AfterSaveChanges(AppDbContext context);
}
