# Generic Repository

## Description
A lightweight and flexible generic repository pattern implementation for .NET applications with Entity Framework Core. Simplifies CRUD operations and query management.

## Installation
```bash
dotnet add package CD.GenericRepository
```

## Features
* Generic Repository Pattern implementation
* Async/Sync operation support
* Tracking/Non-tracking queries
* Built-in Unit of Work pattern
* Flexible entity queries
* Bulk operation support
* Cancellation token support


## Quick Start
1. First, implement your DbContext:
```csharp
public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
}
```

2. Create your repository:
```csharp
public interface IUserRepository : IRepository<User>
{
}

public class UserRepository : Repository<User, AppDbContext>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }
}
```

3. Use in your service what do you need:
```csharp
public class UserService
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUserRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<User?> GetUserAsync(int id)
    {
        return await _repository.GetByExpressionAsync(u => u.Id == id);
    }

    public async Task CreateUserAsync(User user)
    {
        await _repository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }
}
```

4. Register services:
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
```

## Available Operations
* IQueryable<TEntity> GetAll();
* IQueryable<TEntity> GetAllWithTracking();
* IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression);
* IQueryable<TEntity> WhereWithTracking(Expression<Func<TEntity, bool>> expression);
* TEntity First(Expression<Func<TEntity, bool>> expression, bool isTrackingActive = true);
* TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression, bool isTrackingActive = true);
* Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default, bool isTrackingActive = true);
* Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default, bool isTrackingActive = true);
* Task<TEntity> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
* Task<TEntity> GetByExpressionWithTrackingAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
* Task<TEntity> GetFirstAsync(CancellationToken cancellationToken = default);
* Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
* bool Any(Expression<Func<TEntity, bool>> expression);
* TEntity GetByExpression(Expression<Func<TEntity, bool>> expression);
* TEntity GetByExpressionWithTracking(Expression<Func<TEntity, bool>> expression);
* TEntity GetFirst();
* Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
* void Add(TEntity entity);
* Task AddRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default);
* void AddRange(ICollection<TEntity> entities);
* void Update(TEntity entity);
* void UpdateRange(ICollection<TEntity> entities);
* Task DeleteByIdAsync(string id);
* Task DeleteByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
* void Delete(TEntity entity);
* void DeleteRange(ICollection<TEntity> entities);
* IQueryable<KeyValuePair<bool, int>> CountBy(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

## Requirements
* .NET 9.0 or later
* Entity Framework Core 9.0 or later

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE.txt) file for details.
