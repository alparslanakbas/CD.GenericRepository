# Result Pattern

## Description
A lightweight and flexible Result pattern implementation for .NET applications. Simplifies API response handling and error management.

## Installation
```bash
dotnet add package CD.Results
```

## Features
* Generic Result Pattern implementation
* Built-in HTTP Status Code support
* Error message management
* JSON serialization support
* Easy to use Success/Failure patterns
* Common HTTP status code helpers (NotFound, BadRequest, etc.)

## Quick Start
1. Return a success result:
```csharp
public async Task<Result<UserDto>> GetUserAsync(int id)
{
    var user = await _repository.GetByIdAsync(id);
    var userDto = _mapper.Map<UserDto>(user);
    return Result<UserDto>.Success(userDto);
}
```

2. Return a failure result:
```csharp
public async Task<Result<UserDto>> GetUserAsync(int id)
{
    var user = await _repository.GetByIdAsync(id);
    
    if (user is null)
        return Result<UserDto>.NotFound($"User not found with id: {id}");

    var userDto = _mapper.Map<UserDto>(user);
    return Result<UserDto>.Success(userDto);
}
```

3. Use in API Controller:
```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _userService.GetUserAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }
}
```

## Response Examples
### Success Response
```json
{
    "data": {
        "id": 1,
        "name": "Alparslan Akbas",
        "email": "alparslan@example.com"
    },
    "isSuccessful": true,
    "statusCode": 200,
    "errorMessages": null
}
```

### Failure Response
```json
{
    "data": null,
    "isSuccessful": false,
    "statusCode": 404,
    "errorMessages": ["User not found with id: 1"]
}
```

## Common Methods
```csharp
// Success results
Result<T>.Success(data);

// Failure results
Result<T>.Failure(HttpStatusCode.BadRequest, "Invalid request");
Result<T>.NotFound("Resource not found");
Result<T>.BadRequest("Validation failed");
Result<T>.Unauthorized("Access denied");
Result<T>.Forbidden("Not allowed");
```

## Requirements
* .NET 9.0 or later

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE.txt) file for details.