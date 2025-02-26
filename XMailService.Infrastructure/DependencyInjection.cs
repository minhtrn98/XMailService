using Microsoft.AspNetCore.Connections;
using XMailService.Infrastructure.Persistence;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace XMailService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
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

        return services;
    }
}
