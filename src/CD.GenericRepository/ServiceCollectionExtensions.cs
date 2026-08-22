using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CD.GenericRepository;

/// <summary>Registers repositories and unit-of-work services for a single EF Core context.</summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGenericRepositories<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddScoped<DbContext>(provider => provider.GetRequiredService<TContext>());
        services.TryAddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.TryAddScoped(typeof(IReadRepository<>), typeof(Repository<>));
        services.TryAddScoped<EfUnitOfWork<TContext>>();
        services.TryAddScoped<IUnitOfWork>(provider => provider.GetRequiredService<EfUnitOfWork<TContext>>());
        services.TryAddScoped<ITransactionalUnitOfWork>(provider =>
            provider.GetRequiredService<EfUnitOfWork<TContext>>());

        return services;
    }
}
