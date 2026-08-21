# Changelog

All notable changes to this project are documented here.

## 3.0.0

### Added

- New `CD.GenericRepository` primary package that includes `CD.Results` transitively.
- .NET 8, .NET 9 and .NET 10 multi-targeting.
- `IReadRepository<TEntity>` and explicit `Query`, `GetAsync`, `ListAsync`, `ExistsAsync`, and `CountAsync` APIs.
- Safe `RemoveByIdAsync` and `RemoveFirstAsync` operations.
- Non-generic `Result`, `Created`, `Conflict`, and `Match` helpers.
- SQLite-backed integration tests, CI, package validation, symbols, and automated release workflow.

### Changed

- Nullable return types now reflect missing entities correctly.
- Read operations are no-tracking by default.
- Cancellation tokens are forwarded by all async queries.
- Exception details are hidden by `ToResultAsync` unless an explicit mapping function is supplied.

### Deprecated

- The 2.x repository method names. Compatibility members remain for this major release.
- The `CD.Generic.Repository` package ID. It now acts as a migration package for `CD.GenericRepository`.

### Fixed

- Removing a missing entity no longer passes `null` to EF Core.
- Duplicate predicates and ignored cancellation tokens in async queries.
- Broken namespaces and dependency registration in the sample API.
