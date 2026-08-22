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

    /// <summary>Projects and materializes only the fields required by the caller.</summary>
    Task<IReadOnlyList<TResult>> ProjectAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>Returns one ordered page projected directly by the database provider.</summary>
    Task<PagedList<TResult>> PageAsync<TResult, TOrderKey>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, TOrderKey>> orderBy,
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool descending = false,
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
