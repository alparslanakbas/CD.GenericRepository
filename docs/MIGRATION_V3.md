# Migrating to version 3

Version 3 introduces `CD.GenericRepository` as the primary one-reference package and keeps `CD.Results` independently reusable.

## Package references

For a new project, use only:

```xml
<PackageReference Include="CD.GenericRepository" Version="3.0.0" />
```

This reference brings in `CD.Results` transitively. Existing applications can update `CD.Generic.Repository` to 3.0.0 first; the compatibility package forwards to the new package.

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
- Success requires a 2xx status code; failure requires a non-2xx status code and at least one message.
- `ToResultAsync` no longer exposes exception messages by default.
- `OperationCanceledException` is propagated rather than converted into an HTTP 500 result.
