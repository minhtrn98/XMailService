using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using XMailService.Application.Interfaces;
using XMailService.Application.Repositories;
using XMailService.Infrastructure.Interceptors;
using XMailService.Infrastructure.Persistence;
using XMailService.Infrastructure.Repositories;
using XMailService.Infrastructure.Services;

namespace XMailService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);

        services.AddScoped<IMailTemplateRepository, MailTemplateRepository>();
        services.AddScoped<IMailSignatureRepository, MailSignatureRepository>();

        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services.AddScoped<IDateTimeProvider, SystemDateTimeProvider>();

        services.AddScoped<IUnitOfWorkInterceptor, AuditableEntityInterceptor>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPooledDbContextFactory<AppDbContext>((sp, options) => options
            .UseNpgsql(
                connectionString: configuration.GetConnectionString("Default"),
                npgsqlOptionsAction: optionBuilder =>
                    optionBuilder
                    .ExecutionStrategy(dependencies =>
                        new NpgsqlRetryingExecutionStrategy(
                            dependencies: dependencies,
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(1),
                            errorCodesToAdd: null)
                    ).MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name)
            )
        );
        services.AddScoped<AppDbContextScopedFactory>();
        services.AddScoped(sp => sp.GetRequiredService<AppDbContextScopedFactory>().CreateDbContext());
        services.AddScoped<IDbConnectionFactory, AppDbConnection>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
