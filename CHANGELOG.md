# Changelog

All notable changes to this project are documented here.

## 3.0.0-preview.1

### Added

- New `CD.GenericRepository` primary package that includes `CD.Results` transitively.
- .NET 8, .NET 9 and .NET 10 multi-targeting.
- `IReadRepository<TEntity>` and explicit `Query`, `GetAsync`, `ListAsync`, `ExistsAsync`, and `CountAsync` APIs.
- Safe `RemoveByIdAsync` and `RemoveFirstAsync` operations.
- Database-side projection and ordered `PagedList<T>` queries.
- Composite-key deletion through `RemoveByKeyAsync`.
- `EfUnitOfWork<TContext>`, transaction boundaries, and one-line dependency injection registration.
- Structured, transport-independent Result errors with stable codes and classifications.
- Optional `CD.Results.AspNetCore` mappings for controllers and Minimal APIs.
- Public API compatibility analysis that rejects unreviewed surface changes.
- SQLite-backed integration tests, CI, package validation, symbols, and automated release workflow.

### Changed

- Nullable return types now reflect missing entities correctly.
- Read operations are no-tracking by default.
- Cancellation tokens are forwarded by all async queries.
- Exception details are hidden by `ToResultAsync` unless an explicit mapping function is supplied.
- HTTP status handling moved out of `CD.Results` and into `CD.Results.AspNetCore`.
- Release publishing uses short-lived NuGet credentials through trusted publishing.

### Deprecated

- The 2.x repository method names. Compatibility members remain for this major release.
- The `CD.Generic.Repository` package ID. It now acts as a migration package for `CD.GenericRepository`.

### Fixed

- Removing a missing entity no longer passes `null` to EF Core.
- Duplicate predicates and ignored cancellation tokens in async queries.
- Broken namespaces and dependency registration in the sample API.
