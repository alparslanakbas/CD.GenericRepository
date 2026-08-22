using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace CD.GenericRepository;

/// <summary>Provides a generic Entity Framework Core repository implementation.</summary>
public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    private readonly DbSet<TEntity> _entities;

    public Repository(DbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        Context = context;
        _entities = context.Set<TEntity>();
    }

    protected DbContext Context { get; }

    public IQueryable<TEntity> Query(bool asTracking = false)
        => asTracking ? _entities : _entities.AsNoTracking();

    public Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return Query(asTracking).FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = Query(asTracking);

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<TResult>> ProjectAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(selector);
        IQueryable<TEntity> query = Query();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query
            .Select(selector)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<PagedList<TResult>> PageAsync<TResult, TOrderKey>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, TOrderKey>> orderBy,
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool descending = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(orderBy);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageNumber, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        IQueryable<TEntity> query = Query();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        int totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        IOrderedQueryable<TEntity> orderedQuery = descending
            ? query.OrderByDescending(orderBy)
            : query.OrderBy(orderBy);

        List<TResult> items = await orderedQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(selector)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PagedList<TResult>(items, pageNumber, pageSize, totalCount);
    }

    public Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return _entities.AnyAsync(predicate, cancellationToken);
    }

    public Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
        => predicate is null
            ? _entities.CountAsync(cancellationToken)
            : _entities.CountAsync(predicate, cancellationToken);

    public void Add(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _entities.Add(entity);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _entities.AddAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _entities.AddRange(entities);
    }

    public Task AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        return _entities.AddRangeAsync(entities, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _entities.Update(entity);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _entities.UpdateRange(entities);
    }

    public void Remove(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _entities.Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _entities.RemoveRange(entities);
    }

    public async Task<bool> RemoveByIdAsync<TKey>(
        TKey id,
        CancellationToken cancellationToken = default)
        where TKey : notnull
    {
        return await RemoveByKeyAsync([id], cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> RemoveByKeyAsync(
        IReadOnlyList<object> keyValues,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keyValues);

        if (keyValues.Count == 0)
        {
            throw new ArgumentException("At least one key value is required.", nameof(keyValues));
        }

        object?[] keys = keyValues.Cast<object?>().ToArray();
        TEntity? entity = await _entities.FindAsync(keys, cancellationToken).ConfigureAwait(false);

        if (entity is null)
        {
            return false;
        }

        _entities.Remove(entity);
        return true;
    }

    public async Task<bool> RemoveFirstAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        TEntity? entity = await _entities
            .FirstOrDefaultAsync(predicate, cancellationToken)
            .ConfigureAwait(false);

        if (entity is null)
        {
            return false;
        }

        _entities.Remove(entity);
        return true;
    }

    [Obsolete("Use Query() instead. This member will be removed in the next major version.")]
    public IQueryable<TEntity> GetAll() => Query();

    [Obsolete("Use Query(asTracking: true) instead. This member will be removed in the next major version.")]
    public IQueryable<TEntity> GetAllWithTracking() => Query(asTracking: true);

    [Obsolete("Use Query().Where(predicate) instead. This member will be removed in the next major version.")]
    public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        => Query().Where(expression);

    [Obsolete("Use Query(asTracking: true).Where(predicate) instead. This member will be removed in the next major version.")]
    public IQueryable<TEntity> WhereWithTracking(Expression<Func<TEntity, bool>> expression)
        => Query(asTracking: true).Where(expression);

    [Obsolete("Use Query(asTracking).First(predicate) instead. This member will be removed in the next major version.")]
    public TEntity First(Expression<Func<TEntity, bool>> expression, bool isTrackingActive = true)
        => Query(isTrackingActive).First(expression);

    [Obsolete("Use Query(asTracking).FirstOrDefault(predicate) instead. This member will be removed in the next major version.")]
    public TEntity? FirstOrDefault(
        Expression<Func<TEntity, bool>> expression,
        bool isTrackingActive = true)
        => Query(isTrackingActive).FirstOrDefault(expression);

    [Obsolete("Use GetAsync(predicate, asTracking, cancellationToken) instead. This member will be removed in the next major version.")]
    public Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default,
        bool isTrackingActive = true)
        => GetAsync(expression, isTrackingActive, cancellationToken);

    [Obsolete("Use Query(asTracking).FirstAsync(predicate, cancellationToken) instead. This member will be removed in the next major version.")]
    public Task<TEntity> FirstAsync(
        Expression<Func<TEntity, bool>> expression,
        bool isTrackingActive = true,
        CancellationToken cancellationToken = default)
        => Query(isTrackingActive).FirstAsync(expression, cancellationToken);

    [Obsolete("Use GetAsync(predicate, cancellationToken: cancellationToken) instead. This member will be removed in the next major version.")]
    public Task<TEntity?> GetByExpressionAsync(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default)
        => GetAsync(expression, cancellationToken: cancellationToken);

    [Obsolete("Use GetAsync(predicate, asTracking: true, cancellationToken) instead. This member will be removed in the next major version.")]
    public Task<TEntity?> GetByExpressionWithTrackingAsync(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default)
        => GetAsync(expression, asTracking: true, cancellationToken);

    [Obsolete("Use Query().FirstOrDefaultAsync(cancellationToken) instead. This member will be removed in the next major version.")]
    public Task<TEntity?> GetFirstAsync(CancellationToken cancellationToken = default)
        => Query().FirstOrDefaultAsync(cancellationToken);

    [Obsolete("Use Query().FirstOrDefault(predicate) instead. This member will be removed in the next major version.")]
    public TEntity? GetByExpression(Expression<Func<TEntity, bool>> expression)
        => Query().FirstOrDefault(expression);

    [Obsolete("Use Query(asTracking: true).FirstOrDefault(predicate) instead. This member will be removed in the next major version.")]
    public TEntity? GetByExpressionWithTracking(Expression<Func<TEntity, bool>> expression)
        => Query(asTracking: true).FirstOrDefault(expression);

    [Obsolete("Use Query().FirstOrDefault() instead. This member will be removed in the next major version.")]
    public TEntity? GetFirst() => Query().FirstOrDefault();

    [Obsolete("Use ExistsAsync(predicate, cancellationToken) instead. This member will be removed in the next major version.")]
    public Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default)
        => ExistsAsync(expression, cancellationToken);

    [Obsolete("Use Query().Any(predicate) instead. This member will be removed in the next major version.")]
    public bool Any(Expression<Func<TEntity, bool>> expression) => _entities.Any(expression);

    [Obsolete("Use RemoveByIdAsync(id) instead. This member will be removed in the next major version.")]
    public async Task DeleteByIdAsync<TKey>(TKey id)
        where TKey : struct
        => _ = await RemoveByIdAsync(id).ConfigureAwait(false);

    [Obsolete("Use RemoveFirstAsync(predicate, cancellationToken) instead. This member will be removed in the next major version.")]
    public async Task DeleteByExpressionAsync(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default)
        => _ = await RemoveFirstAsync(expression, cancellationToken).ConfigureAwait(false);

    [Obsolete("Use Remove(entity) instead. This member will be removed in the next major version.")]
    public void Delete(TEntity entity) => Remove(entity);

    [Obsolete("Use RemoveRange(entities) instead. This member will be removed in the next major version.")]
    public void DeleteRange(IEnumerable<TEntity> entities) => RemoveRange(entities);

    [Obsolete("Use Query().GroupBy(predicate) for custom aggregations. This member will be removed in the next major version.")]
    public IQueryable<KeyValuePair<bool, int>> CountBy(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(expression);

        return _entities
            .GroupBy(expression)
            .Select(group => new KeyValuePair<bool, int>(group.Key, group.Count()));
    }
}

/// <summary>Preserves the strongly typed DbContext constructor used by earlier releases.</summary>
public class Repository<TEntity, TContext> : Repository<TEntity>
    where TEntity : class
    where TContext : DbContext
{
    public Repository(TContext context)
        : base(context)
    {
        TypedContext = context;
    }

    protected TContext TypedContext { get; }
}
