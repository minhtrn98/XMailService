using Microsoft.EntityFrameworkCore;

namespace XMailService.Infrastructure.Persistence;

public sealed class AppDbContextScopedFactory(
    IDbContextFactory<AppDbContext> pooledFactory
    ) : IDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext()
    {
        AppDbContext context = pooledFactory.CreateDbContext();
        return context;
    }
}
