# CD.Results

A lightweight Result pattern that is independent from HTTP, databases, and UI frameworks.

`CD.Results` is installed automatically by `CD.GenericRepository`, but it can also be used independently:

```bash
dotnet add package CD.Results --prerelease
```

## Usage

```csharp
Result<User> success = Result<User>.Success(user);
Result<User> missing = Result<User>.NotFound("User not found", "user.not_found");
Result<User> invalid = Result<User>.Validation("Email is invalid", "user.email.invalid");
Result<User> conflict = Result<User>.Conflict("Email already exists", "user.email.conflict");
```

Errors contain a stable code, a human-readable message, and a transport-independent type:

```csharp
ResultError error = result.Errors![0];
logger.LogWarning("{Code}: {Message}", error.Code, error.Message);
```

For commands that do not return data:

```csharp
Result success = Result.Success();
Result failure = Result.Validation("Validation failed");
```

Map both paths without reading an invalid value:

```csharp
string message = result.Match(
    value => $"Loaded {value?.Name}",
    errors => errors[0].Message);
```

`ToResultAsync` propagates cancellation and hides exception details by default:

```csharp
Result<User> result = await userTask.ToResultAsync();
```

ASP.NET Core applications can install `CD.Results.AspNetCore` to map results to controller or Minimal API responses. Targets .NET 8, .NET 9 and .NET 10.
