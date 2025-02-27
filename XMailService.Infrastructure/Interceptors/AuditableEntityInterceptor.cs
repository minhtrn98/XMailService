using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using XMailService.Application.Interfaces;
using XMailService.Application.Models;
using XMailService.Domain.Entities.Commons;
using XMailService.Infrastructure.Persistence;

namespace XMailService.Infrastructure.Interceptors;

public sealed class AuditableEntityInterceptor(ICurrentUserProvider currentUserProvider)
    : IUnitOfWorkInterceptor
{
    public int Priority => int.MinValue;

    public Task BeforeSaveChanges(AppDbContext context)
    {
        CurrentUser currentUser = currentUserProvider.GetCurrentUser();
        UpdateSoftDeleted(context);
        AddAuditData(context, currentUser);

        return Task.CompletedTask;
    }

    public Task AfterSaveChanges(AppDbContext context) => Task.CompletedTask;

    private static void AddAuditData(DbContext dbContext, CurrentUser currentUser)
    {
        IEnumerable<EntityEntry<IAuditEntity>> entries = dbContext.ChangeTracker.Entries<IAuditEntity>()
                    .Where(e => e is { State: EntityState.Added or EntityState.Modified });

        foreach (EntityEntry<IAuditEntity> entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Entity.CreatedBy = currentUser.Id;
                entityEntry.Entity.CreatedDate = DateTime.UtcNow;
            }
            else
            {
                entityEntry.Entity.UpdatedBy = currentUser.Id;
                entityEntry.Entity.UpdatedDate = DateTime.UtcNow;
            }
        }
    }

    private static void UpdateSoftDeleted(DbContext dbContext)
    {
        IEnumerable<EntityEntry<ISoftDelete>> deleteEntries = dbContext.ChangeTracker.Entries<ISoftDelete>()
                    .Where(e => e is { State: EntityState.Deleted });

        foreach (EntityEntry<ISoftDelete> deleteEntry in deleteEntries)
        {
            deleteEntry.Entity.SoftDeleted = true;
            deleteEntry.State = EntityState.Modified;
        }
    }
}
