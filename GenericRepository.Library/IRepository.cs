using System.Linq.Expressions;

namespace CD.GenericRepository
{
    /// <summary>
    /// Provides a generic repository pattern implementation for entity operations.
    /// This interface encapsulates the CRUD and query operations for an entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type this repository works with</typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        #region Query Operations

        /// <summary>
        /// Retrieves all entities from the database without change tracking.
        /// </summary>
        /// <remarks>
        /// This method is optimal for read-only scenarios as it doesn't track changes.
        /// The returned IQueryable can be further filtered, ordered, or projected.
        /// </remarks>
        /// <returns>An IQueryable instance containing all entities</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Retrieves all entities from the database with change tracking enabled.
        /// </summary>
        /// <remarks>
        /// Use this method when you need to modify the retrieved entities.
        /// Change tracking has a performance impact, only use when necessary.
        /// </remarks>
        /// <returns>An IQueryable instance containing all tracked entities</returns>
        IQueryable<TEntity> GetAllWithTracking();

        /// <summary>
        /// Filters entities based on a predicate without change tracking.
        /// </summary>
        /// <param name="expression">The filtering expression</param>
        /// <returns>An IQueryable of filtered entities</returns>
        /// <example>
        /// <code>
        /// var activeUsers = repository.Where(user => user.IsActive);
        /// var adultUsers = repository.Where(user => user.Age >= 18);
        /// </code>
        /// </example>
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// Filters entities based on a predicate with change tracking enabled.
        /// </summary>
        /// <param name="expression">The filtering expression</param>
        /// <returns>An IQueryable of filtered entities with change tracking enabled</returns>
        /// <remarks>
        /// Use this method when you need to modify the filtered entities.
        /// </remarks>
        IQueryable<TEntity> WhereWithTracking(Expression<Func<TEntity, bool>> expression);

        #endregion

        #region First Operations

        /// <summary>
        /// Gets the first entity matching the specified condition.
        /// </summary>
        /// <param name="expression">The condition to match</param>
        /// <param name="isTrackingActive">Whether to enable change tracking</param>
        /// <returns>The first matching entity</returns>
        /// <exception cref="InvalidOperationException">Thrown when no entity matches the condition</exception>
        TEntity First(Expression<Func<TEntity, bool>> expression, bool isTrackingActive = true);

        /// <summary>
        /// Gets the first entity matching the specified condition or null if none found.
        /// </summary>
        /// <param name="expression">The condition to match</param>
        /// <param name="isTrackingActive">Whether to enable change tracking</param>
        /// <returns>The first matching entity or null</returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression, bool isTrackingActive = true);

        /// <summary>
        /// Asynchronously gets the first entity matching the specified condition or null if none found.
        /// </summary>
        /// <param name="expression">The condition to match</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <param name="isTrackingActive">Whether to enable change tracking</param>
        /// <returns>A task containing the first matching entity or null</returns>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default, bool isTrackingActive = true);

        /// <summary>
        /// Asynchronously gets the first entity matching the specified condition.
        /// </summary>
        /// <param name="expression">The condition to match</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <param name="isTrackingActive">Whether to enable change tracking</param>
        /// <returns>A task containing the first matching entity</returns>
        /// <exception cref="InvalidOperationException">Thrown when no entity matches the condition</exception>
        Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> expression, bool isTrackingActive = true, CancellationToken cancellationToken = default);

        #endregion

        #region Get Operations

        /// <summary>
        /// Asynchronously retrieves an entity based on the specified expression without tracking.
        /// </summary>
        /// <param name="expression">The expression to filter the entity</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>A task containing the matching entity or null</returns>
        Task<TEntity> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously retrieves an entity based on the specified expression with tracking enabled.
        /// </summary>
        /// <param name="expression">The expression to filter the entity</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>A task containing the matching entity or null</returns>
        Task<TEntity> GetByExpressionWithTrackingAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously retrieves the first entity from the database.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>A task containing the first entity or null if the database is empty</returns>
        Task<TEntity> GetFirstAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves an entity based on the specified expression without tracking.
        /// </summary>
        /// <param name="expression">The expression to filter the entity</param>
        /// <returns>The matching entity or null</returns>
        TEntity GetByExpression(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// Retrieves an entity based on the specified expression with tracking enabled.
        /// </summary>
        /// <param name="expression">The expression to filter the entity</param>
        /// <returns>The matching entity or null</returns>
        TEntity GetByExpressionWithTracking(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// Retrieves the first entity from the database.
        /// </summary>
        /// <returns>The first entity or null if the database is empty</returns>
        TEntity GetFirst();

        #endregion

        #region Existence Checks

        /// <summary>
        /// Asynchronously checks if any entity matches the specified condition.
        /// </summary>
        /// <param name="expression">The condition to check</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>A task containing true if any entity matches; otherwise, false</returns>
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if any entity matches the specified condition.
        /// </summary>
        /// <param name="expression">The condition to check</param>
        /// <returns>True if any entity matches; otherwise, false</returns>
        bool Any(Expression<Func<TEntity, bool>> expression);

        #endregion

        #region Add Operations

        /// <summary>
        /// Asynchronously adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>A task representing the asynchronous operation</returns>
        /// <exception cref="ArgumentNullException">Thrown when entity is null</exception>
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null</exception>
        void Add(TEntity entity);

        /// <summary>
        /// Asynchronously adds multiple entities to the database.
        /// </summary>
        /// <param name="entities">The collection of entities to add</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>A task representing the asynchronous operation</returns>
        /// <exception cref="ArgumentNullException">Thrown when entities collection is null</exception>
        Task AddRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds multiple entities to the database.
        /// </summary>
        /// <param name="entities">The collection of entities to add</param>
        /// <exception cref="ArgumentNullException">Thrown when entities collection is null</exception>
        void AddRange(ICollection<TEntity> entities);

        #endregion

        #region Update Operations

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null</exception>
        void Update(TEntity entity);

        /// <summary>
        /// Updates multiple existing entities in the database.
        /// </summary>
        /// <param name="entities">The collection of entities to update</param>
        /// <exception cref="ArgumentNullException">Thrown when entities collection is null</exception>
        void UpdateRange(ICollection<TEntity> entities);

        #endregion

        #region Delete Operations

        /// <summary>
        /// Deletes an entity by its ID
        /// </summary>
        /// <typeparam name="TKey">The type of the ID field</typeparam>
        /// <param name="id">The ID value of the entity to delete</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task DeleteByIdAsync<TKey>(TKey id) where TKey : struct;

        /// <summary>
        /// Asynchronously deletes entities matching the specified condition.
        /// </summary>
        /// <param name="expression">The condition to match entities for deletion</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task DeleteByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an entity from the database.
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null</exception>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes multiple entities from the database.
        /// </summary>
        /// <param name="entities">The collection of entities to delete</param>
        /// <exception cref="ArgumentNullException">Thrown when entities collection is null</exception>
        void DeleteRange(ICollection<TEntity> entities);

        #endregion

        #region Count Operations

        /// <summary>
        /// Groups and counts entities based on a boolean expression.
        /// </summary>
        /// <param name="expression">The expression to group by</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>An IQueryable containing key-value pairs of the boolean result and count</returns>
        /// <example>
        /// <code>
        /// var usersByActiveStatus = repository.CountBy(user => user.IsActive);
        /// </code>
        /// </example>
        IQueryable<KeyValuePair<bool, int>> CountBy(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

        #endregion
    }
}