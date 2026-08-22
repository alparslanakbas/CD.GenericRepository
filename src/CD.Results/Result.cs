using System.Text.Json.Serialization;

namespace CD.Results;

/// <summary>Represents an operation that does not return a value.</summary>
public sealed class Result : BaseResult
{
    private Result(bool isSuccessful, IEnumerable<ResultError>? errors = null)
        : base(isSuccessful, errors)
    {
    }

    public static Result Success() => new(true);

    public static Result Failure(string message, string code = "failure")
        => Failure(new ResultError(code, message));

    public static Result Failure(ResultError error)
        => new(false, [error ?? throw new ArgumentNullException(nameof(error))]);

    public static Result Failure(IEnumerable<ResultError> errors)
        => new(false, errors ?? throw new ArgumentNullException(nameof(errors)));

    public static Result Validation(string message, string code = "validation")
        => Failure(new ResultError(code, message, ResultErrorType.Validation));

    public static Result NotFound(string message = "Resource not found", string code = "not_found")
        => Failure(new ResultError(code, message, ResultErrorType.NotFound));

    public static Result Unauthorized(string message = "Unauthorized access", string code = "unauthorized")
        => Failure(new ResultError(code, message, ResultErrorType.Unauthorized));

    public static Result Forbidden(string message = "Forbidden access", string code = "forbidden")
        => Failure(new ResultError(code, message, ResultErrorType.Forbidden));

    public static Result Conflict(string message, string code = "conflict")
        => Failure(new ResultError(code, message, ResultErrorType.Conflict));

    public static Result Unexpected(string message = "An unexpected error occurred.", string code = "unexpected")
        => Failure(new ResultError(code, message, ResultErrorType.Unexpected));

    public TResult Match<TResult>(Func<TResult> onSuccess, Func<IReadOnlyList<ResultError>, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        return IsSuccessful ? onSuccess() : onFailure(Errors!);
    }
}

/// <summary>Represents an operation that returns a value when successful.</summary>
public sealed class Result<T> : BaseResult
{
    private Result(T? data, bool isSuccessful, IEnumerable<ResultError>? errors = null)
        : base(isSuccessful, errors)
    {
        Data = data;
    }

    [JsonPropertyName("data")]
    public T? Data { get; }

    public static Result<T> Success(T data) => new(data, true);

    public static Result<T> Failure(string message, string code = "failure")
        => Failure(new ResultError(code, message));

    public static Result<T> Failure(ResultError error)
        => new(default, false, [error ?? throw new ArgumentNullException(nameof(error))]);

    public static Result<T> Failure(IEnumerable<ResultError> errors)
        => new(default, false, errors ?? throw new ArgumentNullException(nameof(errors)));

    public static Result<T> Validation(string message, string code = "validation")
        => Failure(new ResultError(code, message, ResultErrorType.Validation));

    public static Result<T> NotFound(string message = "Resource not found", string code = "not_found")
        => Failure(new ResultError(code, message, ResultErrorType.NotFound));

    public static Result<T> Unauthorized(string message = "Unauthorized access", string code = "unauthorized")
        => Failure(new ResultError(code, message, ResultErrorType.Unauthorized));

    public static Result<T> Forbidden(string message = "Forbidden access", string code = "forbidden")
        => Failure(new ResultError(code, message, ResultErrorType.Forbidden));

    public static Result<T> Conflict(string message, string code = "conflict")
        => Failure(new ResultError(code, message, ResultErrorType.Conflict));

    public static Result<T> Unexpected(string message = "An unexpected error occurred.", string code = "unexpected")
        => Failure(new ResultError(code, message, ResultErrorType.Unexpected));

    public TResult Match<TResult>(
        Func<T?, TResult> onSuccess,
        Func<IReadOnlyList<ResultError>, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        return IsSuccessful ? onSuccess(Data) : onFailure(Errors!);
    }

    public static implicit operator Result<T>(T data) => Success(data);
}
