using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CD.GenericRepository
{
    public class Repository<TEntity, TContext> : IRepository<TEntity>
        where TEntity : class
        where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly DbSet<TEntity> _entities;

        public Repository(TContext context)
        {
            _context = context;
            _entities = _context.Set<TEntity>();
        }

        public void Add(TEntity entity)
        {
            _entities.Add(entity);
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _entities.AddAsync(entity, cancellationToken);
        }

        public void AddRange(ICollection<TEntity> entities)
        {
            _entities.AddRange(entities);
        }

        public async Task AddRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await _entities.AddRangeAsync(entities, cancellationToken);
        }

        public bool Any(Expression<Func<TEntity, bool>> expression)
        {
            return _entities.Any(expression);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            return await _entities.AnyAsync(expression, cancellationToken);
        }

        public IQueryable<KeyValuePair<bool, int>> CountBy(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            return _entities.CountBy(expression);
        }

        public void Delete(TEntity entity)
        {
            _entities.Remove(entity);
        }

        public async Task DeleteByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            _entities.Remove(await _entities.FirstOrDefaultAsync(expression, cancellationToken));
        }

        public async Task DeleteByIdAsync<TKey>(TKey id) where TKey : struct
            => _entities.Remove(await _entities.FindAsync(id));

        public void DeleteRange(ICollection<TEntity> entities)
        {
            _entities.RemoveRange(entities);
        }

        public TEntity First(Expression<Func<TEntity, bool>> expression, bool isTrackingActive = true)
        {
            return isTrackingActive ? _entities.First(expression) : _entities.AsNoTracking().First(expression);
        }

        public Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> expression, bool isTrackingActive = true, CancellationToken cancellationToken = default)
        {
            return isTrackingActive ? _entities.FirstAsync(expression, cancellationToken) : _entities.AsNoTracking().FirstAsync(expression, cancellationToken);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression, bool isTrackingActive = true)
        {
            return isTrackingActive ? _entities.FirstOrDefault(expression) : _entities.AsNoTracking().FirstOrDefault(expression);
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default, bool isTrackingActive = true)
        {
            TEntity entity = isTrackingActive ? await _entities.Where(expression).FirstOrDefaultAsync(expression, cancellationToken) : await _entities.Where(expression).AsNoTracking().FirstOrDefaultAsync(expression, cancellationToken);
            return entity;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _entities.AsNoTracking().AsQueryable();
        }

        public IQueryable<TEntity> GetAllWithTracking()
        {
            return _entities.AsQueryable();
        }

        public TEntity GetByExpression(Expression<Func<TEntity, bool>> expression)
        {
            TEntity entity = _entities.AsNoTracking().FirstOrDefault(expression);
            return entity;
        }

        public async Task<TEntity> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            TEntity entity = await _entities.Where(expression).AsNoTracking().FirstOrDefaultAsync();
            return entity;
        }

        public TEntity GetByExpressionWithTracking(Expression<Func<TEntity, bool>> expression)
        {
            TEntity entity = _entities.Where(expression).FirstOrDefault();
            return entity;
        }

        public async Task<TEntity> GetByExpressionWithTrackingAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            TEntity entity = await _entities.Where(expression).FirstOrDefaultAsync();
            return entity;
        }

        public TEntity GetFirst()
        {
            TEntity entity = _entities.AsNoTracking().FirstOrDefault();
            return entity;
        }

        public async Task<TEntity> GetFirstAsync(CancellationToken cancellationToken = default)
        {
            TEntity entity = await _entities.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            return entity;
        }

        public void Update(TEntity entity)
        {
            _entities.Update(entity);
        }

        public void UpdateRange(ICollection<TEntity> entities)
        {
            _entities.UpdateRange(entities);
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            return _entities.Where(expression).AsNoTracking().AsQueryable();
        }

        public IQueryable<TEntity> WhereWithTracking(Expression<Func<TEntity, bool>> expression)
        {
            return _entities.Where(expression).AsQueryable();
        }
    }
}
