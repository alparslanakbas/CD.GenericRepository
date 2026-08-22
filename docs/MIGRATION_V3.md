# Migrating to version 3

Version 3 introduces `CD.GenericRepository` as the primary one-reference package and keeps `CD.Results` independently reusable.

## Package references

For a new project, use only:

```xml
<PackageReference Include="CD.GenericRepository" Version="3.0.0-preview.1" />
```

This reference brings in `CD.Results` transitively. Existing applications can update `CD.Generic.Repository` to `3.0.0-preview.1` first; the compatibility package forwards to the new package.

## Namespace compatibility

The public namespaces remain unchanged:

```csharp
using CD.GenericRepository;
using CD.Results;
```

## Repository replacements

| 2.x member | 3.x replacement |
| --- | --- |
| `GetAll()` | `Query()` |
| `GetAllWithTracking()` | `Query(asTracking: true)` |
| `Where(predicate)` | `Query().Where(predicate)` |
| `GetByExpressionAsync(predicate)` | `GetAsync(predicate)` |
| `AnyAsync(predicate)` | `ExistsAsync(predicate)` |
| `Delete(entity)` | `Remove(entity)` |
| `DeleteByIdAsync(id)` | `RemoveByIdAsync(id)` |

The old members remain available but are marked obsolete. They are planned for removal in the next major release.

## Nullability

Methods that may not find an entity now correctly return `TEntity?`. Update callers to handle the missing case explicitly.

## Result changes

- Generic and non-generic results are available.
- `CD.Results` no longer depends on HTTP semantics. Errors now have a stable `Code`, `Message`, and `ResultErrorType`.
- Replace `BadRequest(...)` with `Validation(...)` or `Failure(...)`.
- Replace `Created(data)` with `Success(data)` and choose HTTP 201 at the web boundary.
- Web APIs can install `CD.Results.AspNetCore` and call `result.ToActionResult()` or `result.ToHttpResult()`.
- `ToResultAsync` no longer exposes exception messages by default.
- `OperationCanceledException` is propagated rather than converted into a failed result.

## Registration and transactions

Contexts no longer need to implement `IUnitOfWork` directly:

```csharp
builder.Services.AddDbContext<AppDbContext>(options => /* provider */);
builder.Services.AddGenericRepositories<AppDbContext>();
```

Resolve `IUnitOfWork` for saves or `ITransactionalUnitOfWork` when multiple operations must commit atomically.
