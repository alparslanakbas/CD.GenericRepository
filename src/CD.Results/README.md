# CD.Results

A dependency-free Result pattern for explicit success and failure handling.

`CD.Results` is installed automatically by `CD.GenericRepository`, but it can also be used independently:

```bash
dotnet add package CD.Results
```

## Usage

```csharp
Result<User> success = Result<User>.Success(user);
Result<User> created = Result<User>.Created(user);

Result<User> missing = Result<User>.NotFound("User not found");
Result<User> invalid = Result<User>.BadRequest("Email is invalid");
Result<User> conflict = Result<User>.Conflict("Email already exists");
```

For commands that do not return data:

```csharp
Result success = Result.Success();
Result failure = Result.BadRequest("Validation failed");
```

Map both paths without reading an invalid value:

```csharp
IActionResult response = result.Match<IActionResult>(
    value => Ok(value),
    errors => BadRequest(errors));
```

`ToResultAsync` propagates cancellation and hides exception details by default:

```csharp
Result<User> result = await userTask.ToResultAsync();
```

Targets .NET 8, .NET 9 and .NET 10.
