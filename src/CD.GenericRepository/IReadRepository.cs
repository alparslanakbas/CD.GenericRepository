using System.Linq.Expressions;

namespace CD.GenericRepository;

/// <summary>Defines read-only operations for an Entity Framework Core repository.</summary>
public interface IReadRepository<TEntity>
    where TEntity : class
{
    /// <summary>Creates a composable query. Queries are not tracked unless explicitly requested.</summary>
    IQueryable<TEntity> Query(bool asTracking = false);

    /// <summary>Returns the first entity matching the predicate, or <see langword="null"/>.</summary>
    Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool asTracking = false,
        CancellationToken cancellationToken = default);

    /// <summary>Materializes entities matching an optional predicate.</summary>
    Task<IReadOnlyList<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        bool asTracking = false,
        CancellationToken cancellationToken = default);

    /// <summary>Determines whether any entity matches the predicate.</summary>
    Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>Counts entities matching an optional predicate.</summary>
    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
}
