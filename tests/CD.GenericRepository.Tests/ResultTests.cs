using System.Net;
using System.Text.Json;
using CD.Results;

namespace CD.GenericRepository.Tests;

public sealed class ResultTests
{
    [Fact]
    public void Success_contains_data_and_no_errors()
    {
        Result<string> result = Result<string>.Success("ready");

        Assert.True(result.IsSuccessful);
        Assert.False(result.IsFailure);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("ready", result.Data);
        Assert.Null(result.ErrorMessages);
    }

    [Fact]
    public void Failure_normalizes_error_messages()
    {
        Result<string> result = Result<string>.Failure(
            HttpStatusCode.BadRequest,
            [" invalid ", "invalid", "missing"]);

        Assert.True(result.IsFailure);
        Assert.Equal(["invalid", "missing"], result.ErrorMessages);
    }

    [Fact]
    public void Failure_rejects_success_status_codes()
    {
        Assert.Throws<ArgumentException>(
            () => Result<string>.Failure(HttpStatusCode.OK, "invalid"));
    }

    [Fact]
    public void Serialization_preserves_the_public_contract()
    {
        Result<int> result = Result<int>.NotFound("missing");

        string json = JsonSerializer.Serialize(result);

        Assert.Contains("\"isSuccessful\":false", json, StringComparison.Ordinal);
        Assert.Contains("\"statusCode\":404", json, StringComparison.Ordinal);
        Assert.Contains("\"errorMessages\":[\"missing\"]", json, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ToResultAsync_does_not_expose_exception_details_by_default()
    {
        Task<string> operation = Task.FromException<string>(new InvalidOperationException("secret"));

        Result<string> result = await operation.ToResultAsync();

        Assert.Equal(["An unexpected error occurred."], result.ErrorMessages);
    }

    [Fact]
    public async Task ToResultAsync_propagates_cancellation()
    {
        Task<string> operation = Task.FromCanceled<string>(new CancellationToken(canceled: true));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => operation.ToResultAsync());
    }
}
