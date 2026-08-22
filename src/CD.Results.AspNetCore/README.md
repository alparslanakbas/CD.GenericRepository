# CD.Results.AspNetCore

ASP.NET Core mappings for the transport-independent `CD.Results` package.

```bash
dotnet add package CD.Results.AspNetCore
```

Controller example:

```csharp
return result.ToActionResult();
```

Minimal API example:

```csharp
return result.ToHttpResult();
```

Success status codes can be selected explicitly, while failure status codes are mapped from `ResultErrorType`:

```csharp
return result.ToActionResult(StatusCodes.Status201Created);
```
