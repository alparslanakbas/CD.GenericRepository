using System.Net;

namespace CD.Results;

public static class ResultExtensions
{
    /// <summary>Wraps a value in a successful result.</summary>
    public static Result<T> ToResult<T>(this T value) => Result<T>.Success(value);

    /// <summary>
    /// Awaits a task and converts its outcome to a result. Cancellation is always propagated.
    /// </summary>
    public static async Task<Result<T>> ToResultAsync<T>(
        this Task<T> task,
        Func<Exception, string>? errorMessageFactory = null)
    {
        ArgumentNullException.ThrowIfNull(task);

        try
        {
            T value = await task.ConfigureAwait(false);
            return Result<T>.Success(value);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            string message = errorMessageFactory?.Invoke(exception)
                ?? "An unexpected error occurred.";

            return Result<T>.Failure(HttpStatusCode.InternalServerError, message);
        }
    }
}
