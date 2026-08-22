using System.Text.Json;
using System.Text.Json.Serialization;

namespace CD.Results;

/// <summary>Contains the shared state of generic and non-generic results.</summary>
public abstract class BaseResult
{
    protected BaseResult(bool isSuccessful, IEnumerable<ResultError>? errors = null)
    {
        ResultError[] normalizedErrors = errors?
            .Distinct()
            .ToArray() ?? [];

        if (isSuccessful && normalizedErrors.Length > 0)
        {
            throw new ArgumentException("A successful result cannot contain errors.", nameof(errors));
        }

        if (!isSuccessful && normalizedErrors.Length == 0)
        {
            throw new ArgumentException("A failed result requires at least one error.", nameof(errors));
        }

        if (normalizedErrors.Select(error => error.Type).Distinct().Skip(1).Any())
        {
            throw new ArgumentException("All errors in a result must have the same type.", nameof(errors));
        }

        IsSuccessful = isSuccessful;
        Errors = normalizedErrors.Length == 0 ? null : normalizedErrors;
    }

    [JsonPropertyName("isSuccessful")]
    public bool IsSuccessful { get; }

    [JsonIgnore]
    public bool IsFailure => !IsSuccessful;

    [JsonPropertyName("errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<ResultError>? Errors { get; }

    [JsonIgnore]
    public ResultErrorType? ErrorType => Errors?[0].Type;

    /// <summary>Provides the message-only view used by versions 1 and 2.</summary>
    [Obsolete("Use Errors to access structured error codes, messages, and types.")]
    [JsonIgnore]
    public IReadOnlyList<string>? ErrorMessages => Errors?
        .Select(error => error.Message)
        .ToArray();

    public override string ToString()
        => JsonSerializer.Serialize(this, GetType(), JsonOptions);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };
}
