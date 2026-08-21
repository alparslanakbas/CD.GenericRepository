# CD.Generic.Repository compatibility package

This package keeps existing `CD.Generic.Repository` installations working while the project moves to the new package name.

For new projects, install the main package instead:

```bash
dotnet add package CD.GenericRepository
```

The `CD.GenericRepository` package also brings in `CD.Results`, so one package reference provides both repository and Result APIs.
