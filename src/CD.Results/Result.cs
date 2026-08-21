using System.Net;
using System.Text.Json.Serialization;

namespace CD.Results;

/// <summary>Represents an operation that does not return a value.</summary>
public sealed class Result : BaseResult
{
    private Result(bool isSuccessful, HttpStatusCode statusCode, IEnumerable<string>? errorMessages = null)
        : base(isSuccessful, statusCode, errorMessages)
    {
    }

    public static Result Success(HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(true, statusCode);

    public static Result Failure(HttpStatusCode statusCode, string errorMessage)
        => new(false, statusCode, [errorMessage]);

    public static Result Failure(HttpStatusCode statusCode, IEnumerable<string> errorMessages)
        => new(false, statusCode, errorMessages);

    public static Result NotFound(string message = "Resource not found")
        => Failure(HttpStatusCode.NotFound, message);

    public static Result BadRequest(string message)
        => Failure(HttpStatusCode.BadRequest, message);

    public static Result Unauthorized(string message = "Unauthorized access")
        => Failure(HttpStatusCode.Unauthorized, message);

    public static Result Forbidden(string message = "Forbidden access")
        => Failure(HttpStatusCode.Forbidden, message);

    public static Result Conflict(string message)
        => Failure(HttpStatusCode.Conflict, message);
}

/// <summary>Represents an operation that returns a value when successful.</summary>
public sealed class Result<T> : BaseResult
{
    private Result(T? data, bool isSuccessful, HttpStatusCode statusCode, IEnumerable<string>? errorMessages = null)
        : base(isSuccessful, statusCode, errorMessages)
    {
        Data = data;
    }

    [JsonPropertyName("data")]
    public T? Data { get; }

    public static Result<T> Success(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(data, true, statusCode);

    public static Result<T> Created(T data)
        => Success(data, HttpStatusCode.Created);

    public static Result<T> Failure(HttpStatusCode statusCode, string errorMessage)
        => new(default, false, statusCode, [errorMessage]);

    public static Result<T> Failure(HttpStatusCode statusCode, IEnumerable<string> errorMessages)
        => new(default, false, statusCode, errorMessages);

    public static Result<T> NotFound(string message = "Resource not found")
        => Failure(HttpStatusCode.NotFound, message);

    public static Result<T> BadRequest(string message)
        => Failure(HttpStatusCode.BadRequest, message);

    public static Result<T> Unauthorized(string message = "Unauthorized access")
        => Failure(HttpStatusCode.Unauthorized, message);

    public static Result<T> Forbidden(string message = "Forbidden access")
        => Failure(HttpStatusCode.Forbidden, message);

    public static Result<T> Conflict(string message)
        => Failure(HttpStatusCode.Conflict, message);

    public TResult Match<TResult>(
        Func<T?, TResult> onSuccess,
        Func<IReadOnlyList<string>, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        return IsSuccessful
            ? onSuccess(Data)
            : onFailure(ErrorMessages!);
    }

    public static implicit operator Result<T>(T data) => Success(data);
}
