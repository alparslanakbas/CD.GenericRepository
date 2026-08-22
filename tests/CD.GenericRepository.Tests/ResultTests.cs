using System.Text.Json;
using CD.Results;
using CD.Results.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CD.GenericRepository.Tests;

public sealed class ResultTests
{
    [Fact]
    public void Success_contains_data_and_no_errors()
    {
        Result<string> result = Result<string>.Success("ready");

        Assert.True(result.IsSuccessful);
        Assert.False(result.IsFailure);
        Assert.Equal("ready", result.Data);
        Assert.Null(result.Errors);
        Assert.Null(result.ErrorType);
    }

    [Fact]
    public void Failure_normalizes_duplicate_structured_errors()
    {
        ResultError invalid = new("email.invalid", "Email is invalid", ResultErrorType.Validation);
        Result<string> result = Result<string>.Failure([invalid, invalid]);

        ResultError error = Assert.Single(result.Errors!);
        Assert.Equal("email.invalid", error.Code);
        Assert.Equal(ResultErrorType.Validation, result.ErrorType);
    }

    [Fact]
    public void Failure_rejects_mixed_error_types()
    {
        ResultError validation = new("validation", "Invalid", ResultErrorType.Validation);
        ResultError missing = new("missing", "Missing", ResultErrorType.NotFound);

        Assert.Throws<ArgumentException>(() => Result.Failure([validation, missing]));
    }

    [Fact]
    public void Serialization_preserves_the_structured_public_contract()
    {
        Result<int> result = Result<int>.NotFound("User is missing", "user.not_found");

        string json = JsonSerializer.Serialize(result);

        Assert.Contains("\"isSuccessful\":false", json, StringComparison.Ordinal);
        Assert.Contains("\"code\":\"user.not_found\"", json, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("\"message\":\"User is missing\"", json, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("\"type\":\"NotFound\"", json, StringComparison.Ordinal);
        Assert.DoesNotContain("statusCode", json, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(ResultErrorType.Validation, StatusCodes.Status400BadRequest)]
    [InlineData(ResultErrorType.NotFound, StatusCodes.Status404NotFound)]
    [InlineData(ResultErrorType.Conflict, StatusCodes.Status409Conflict)]
    [InlineData(ResultErrorType.Unauthorized, StatusCodes.Status401Unauthorized)]
    [InlineData(ResultErrorType.Forbidden, StatusCodes.Status403Forbidden)]
    [InlineData(ResultErrorType.Unexpected, StatusCodes.Status500InternalServerError)]
    public void AspNetCore_mapping_converts_error_types_to_status_codes(
        ResultErrorType errorType,
        int expectedStatusCode)
    {
        Result result = Result.Failure(new ResultError("test", "Failed", errorType));
        ObjectResult response = Assert.IsType<ObjectResult>(result.ToActionResult());

        Assert.Equal(expectedStatusCode, response.StatusCode);
        Assert.Same(result, response.Value);
    }

    [Fact]
    public void AspNetCore_mapping_accepts_an_explicit_success_status_code()
    {
        Result<int> result = Result<int>.Success(42);
        ObjectResult response = Assert.IsType<ObjectResult>(
            result.ToActionResult(StatusCodes.Status201Created));

        Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
    }

    [Fact]
    public void AspNetCore_mapping_returns_an_empty_response_for_204()
    {
        IActionResult response = Result.Success().ToActionResult(StatusCodes.Status204NoContent);
        Assert.IsType<NoContentResult>(response);
    }

    [Fact]
    public void AspNetCore_mapping_rejects_a_failure_status_for_success()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Result.Success().ToActionResult(StatusCodes.Status400BadRequest));
    }

    [Fact]
    public async Task ToResultAsync_does_not_expose_exception_details_by_default()
    {
        Task<string> operation = Task.FromException<string>(new InvalidOperationException("secret"));
        Result<string> result = await operation.ToResultAsync();

        ResultError error = Assert.Single(result.Errors!);
        Assert.Equal(ResultErrorType.Unexpected, error.Type);
        Assert.Equal("An unexpected error occurred.", error.Message);
    }

    [Fact]
    public async Task ToResultAsync_propagates_cancellation()
    {
        Task<string> operation = Task.FromCanceled<string>(new CancellationToken(canceled: true));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => operation.ToResultAsync());
    }
}
