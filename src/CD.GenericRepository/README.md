# CD.GenericRepository

Modern repository and unit-of-work APIs for Entity Framework Core. Installing this package also installs `CD.Results`, providing both persistence and Result patterns through one package reference.

## Install

```bash
dotnet add package CD.GenericRepository --prerelease
```

## Create a repository

```csharp
public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options);

public interface IUserRepository : IRepository<User>;

public sealed class UserRepository(AppDbContext context)
    : Repository<User, AppDbContext>(context), IUserRepository;

builder.Services.AddGenericRepositories<AppDbContext>();
```

## Read data

```csharp
User? user = await repository.GetAsync(
    candidate => candidate.Id == id,
    cancellationToken: cancellationToken);

IReadOnlyList<User> activeUsers = await repository.ListAsync(
    candidate => candidate.IsActive,
    cancellationToken: cancellationToken);

IQueryable<User> query = repository.Query()
    .Where(candidate => candidate.CreatedAt >= startDate)
    .OrderBy(candidate => candidate.Name);
```

Project only the required columns and return an ordered page:

```csharp
PagedList<UserSummary> page = await repository.PageAsync(
    user => new UserSummary(user.Id, user.Name),
    user => user.Name,
    pageNumber: 1,
    pageSize: 20,
    predicate: user => user.IsActive,
    cancellationToken: cancellationToken);
```

Queries are no-tracking by default. Pass `asTracking: true` when tracked entities are required.

## Write data

```csharp
await repository.AddAsync(user, cancellationToken);
await unitOfWork.SaveChangesAsync(cancellationToken);

bool removed = await repository.RemoveByIdAsync(userId, cancellationToken);
if (removed)
{
    await unitOfWork.SaveChangesAsync(cancellationToken);
}
```

## Transactions

```csharp
await transactionalUnitOfWork.ExecuteInTransactionAsync(async token =>
{
    await userRepository.AddAsync(user, token);
    await auditRepository.AddAsync(auditEntry, token);
});
```

Successful operations are saved and committed once. Exceptions roll the transaction back and remain visible to the caller.

## Target frameworks

- `net8.0` / EF Core 8.0.30+
- `net9.0` / EF Core 9.0.19+
- `net10.0` / EF Core 10.0.11+

The previous 2.x members remain available with deprecation warnings to support incremental migration.
