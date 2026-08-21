# CD.GenericRepository

A small, explicit repository and Result toolkit for Entity Framework Core applications.

Version 3 provides one primary package reference while keeping the Result pattern independently reusable:

| Package | Purpose |
| --- | --- |
| `CD.GenericRepository` | Primary package. Includes repository APIs and a dependency on `CD.Results`. |
| `CD.Results` | Dependency-free Result pattern for projects that do not use EF Core. |
| `CD.Generic.Repository` | Compatibility package for users migrating from versions 1.x and 2.x. |

## Install

```bash
dotnet add package CD.GenericRepository
```

That single reference makes both namespaces available:

```csharp
using CD.GenericRepository;
using CD.Results;
```

## Supported targets

- .NET 8 with EF Core 8
- .NET 9 with EF Core 9
- .NET 10 with EF Core 10

## Quick start

Implement `IUnitOfWork` on your context:

```csharp
public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users => Set<User>();
}
```

Create an application-specific repository:

```csharp
public interface IUserRepository : IRepository<User>;

public sealed class UserRepository(AppDbContext context)
    : Repository<User, AppDbContext>(context), IUserRepository;
```

Register it with dependency injection:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddScoped<IUnitOfWork>(services =>
    services.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<IUserRepository, UserRepository>();
```

Use async, cancellation-aware operations and explicit results:

```csharp
public async Task<Result<User>> GetByIdAsync(
    int id,
    CancellationToken cancellationToken = default)
{
    User? user = await repository.GetAsync(
        candidate => candidate.Id == id,
        cancellationToken: cancellationToken);

    return user is null
        ? Result<User>.NotFound($"User not found with id: {id}")
        : Result<User>.Success(user);
}
```

## Design notes

- Read queries are no-tracking by default. Request tracking explicitly when updating loaded entities.
- Repository changes are persisted only when `IUnitOfWork.SaveChangesAsync` is called.
- Missing entities return `null` or `false`; delete operations do not pass `null` to EF Core.
- Every async query accepts and forwards a `CancellationToken`.
- The 2.x API remains available as obsolete migration members for one major release.

See [the v3 migration guide](docs/MIGRATION_V3.md) and the [sample API](samples/CD.GenericRepository.Api).

## Development

```bash
dotnet restore CD.GenericRepository.sln
dotnet build CD.GenericRepository.sln --configuration Release --no-restore
dotnet test CD.GenericRepository.sln --configuration Release --no-build
dotnet pack CD.GenericRepository.sln --configuration Release --no-build
```

## License

MIT
