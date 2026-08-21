using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CD.Results;

/// <summary>Contains the shared state of generic and non-generic results.</summary>
public abstract class BaseResult
{
    protected BaseResult(
        bool isSuccessful,
        HttpStatusCode statusCode,
        IEnumerable<string>? errorMessages = null)
    {
        bool isSuccessStatusCode = (int)statusCode is >= 200 and <= 299;

        if (isSuccessful != isSuccessStatusCode)
        {
            throw new ArgumentException(
                "Successful results require a 2xx status code and failures require a non-2xx status code.",
                nameof(statusCode));
        }

        string[] errors = errorMessages?
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .Select(message => message.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray() ?? [];

        if (!isSuccessful && errors.Length == 0)
        {
            throw new ArgumentException("A failed result requires at least one error message.", nameof(errorMessages));
        }

        IsSuccessful = isSuccessful;
        StatusCode = statusCode;
        ErrorMessages = errors.Length == 0 ? null : errors;
    }

    [JsonPropertyName("isSuccessful")]
    public bool IsSuccessful { get; }

    [JsonIgnore]
    public bool IsFailure => !IsSuccessful;

    [JsonPropertyName("statusCode")]
    public HttpStatusCode StatusCode { get; }

    [JsonPropertyName("errorMessages")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? ErrorMessages { get; }

    public override string ToString()
        => JsonSerializer.Serialize(this, GetType(), JsonOptions);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };
}
