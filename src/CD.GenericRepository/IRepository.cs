using System.Linq.Expressions;

namespace CD.GenericRepository;

/// <summary>Defines read and write operations for an Entity Framework Core repository.</summary>
public interface IRepository<TEntity> : IReadRepository<TEntity>
    where TEntity : class
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    void Add(TEntity entity);

    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    void AddRange(IEnumerable<TEntity> entities);

    void Update(TEntity entity);

    void UpdateRange(IEnumerable<TEntity> entities);

    void Remove(TEntity entity);

    void RemoveRange(IEnumerable<TEntity> entities);

    Task<bool> RemoveByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
        where TKey : notnull;

    /// <summary>Marks an entity with a single or composite primary key for deletion.</summary>
    Task<bool> RemoveByKeyAsync(
        IReadOnlyList<object> keyValues,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveFirstAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    [Obsolete("Use Query() instead. This member will be removed in the next major version.")]
    IQueryable<TEntity> GetAll();

    [Obsolete("Use Query(asTracking: true) instead. This member will be removed in the next major version.")]
    IQueryable<TEntity> GetAllWithTracking();

    [Obsolete("Use Query().Where(predicate) instead. This member will be removed in the next major version.")]
    IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression);

    [Obsolete("Use Query(asTracking: true).Where(predicate) instead. This member will be removed in the next major version.")]
    IQueryable<TEntity> WhereWithTracking(Expression<Func<TEntity, bool>> expression);

    [Obsolete("Use Query(asTracking).First(predicate) instead. This member will be removed in the next major version.")]
    TEntity First(Expression<Func<TEntity, bool>> expression, bool isTrackingActive = true);

    [Obsolete("Use Query(asTracking).FirstOrDefault(predicate) instead. This member will be removed in the next major version.")]
    TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> expression, bool isTrackingActive = true);

    [Obsolete("Use GetAsync(predicate, asTracking, cancellationToken) instead. This member will be removed in the next major version.")]
    Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default,
        bool isTrackingActive = true);

    [Obsolete("Use Query(asTracking).FirstAsync(predicate, cancellationToken) instead. This member will be removed in the next major version.")]
    Task<TEntity> FirstAsync(
        Expression<Func<TEntity, bool>> expression,
        bool isTrackingActive = true,
        CancellationToken cancellationToken = default);

    [Obsolete("Use GetAsync(predicate, cancellationToken: cancellationToken) instead. This member will be removed in the next major version.")]
    Task<TEntity?> GetByExpressionAsync(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default);

    [Obsolete("Use GetAsync(predicate, asTracking: true, cancellationToken) instead. This member will be removed in the next major version.")]
    Task<TEntity?> GetByExpressionWithTrackingAsync(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default);

    [Obsolete("Use Query().FirstOrDefaultAsync(cancellationToken) instead. This member will be removed in the next major version.")]
    Task<TEntity?> GetFirstAsync(CancellationToken cancellationToken = default);

    [Obsolete("Use Query().FirstOrDefault(predicate) instead. This member will be removed in the next major version.")]
    TEntity? GetByExpression(Expression<Func<TEntity, bool>> expression);

    [Obsolete("Use Query(asTracking: true).FirstOrDefault(predicate) instead. This member will be removed in the next major version.")]
    TEntity? GetByExpressionWithTracking(Expression<Func<TEntity, bool>> expression);

    [Obsolete("Use Query().FirstOrDefault() instead. This member will be removed in the next major version.")]
    TEntity? GetFirst();

    [Obsolete("Use ExistsAsync(predicate, cancellationToken) instead. This member will be removed in the next major version.")]
    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default);

    [Obsolete("Use Query().Any(predicate) instead. This member will be removed in the next major version.")]
    bool Any(Expression<Func<TEntity, bool>> expression);

    [Obsolete("Use RemoveByIdAsync(id) instead. This member will be removed in the next major version.")]
    Task DeleteByIdAsync<TKey>(TKey id)
        where TKey : struct;

    [Obsolete("Use RemoveFirstAsync(predicate, cancellationToken) instead. This member will be removed in the next major version.")]
    Task DeleteByExpressionAsync(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default);

    [Obsolete("Use Remove(entity) instead. This member will be removed in the next major version.")]
    void Delete(TEntity entity);

    [Obsolete("Use RemoveRange(entities) instead. This member will be removed in the next major version.")]
    void DeleteRange(IEnumerable<TEntity> entities);

    [Obsolete("Use Query().GroupBy(predicate) for custom aggregations. This member will be removed in the next major version.")]
    IQueryable<KeyValuePair<bool, int>> CountBy(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default);
}
