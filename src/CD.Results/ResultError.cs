using System.Text.Json.Serialization;

namespace CD.Results;

/// <summary>Describes a machine-readable operation error.</summary>
public sealed record ResultError
{
    public ResultError(string code, string message, ResultErrorType type = ResultErrorType.Failure)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("An error code is required.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("An error message is required.", nameof(message));
        }

        Code = code.Trim();
        Message = message.Trim();
        Type = type;
    }

    [JsonPropertyName("code")]
    public string Code { get; }

    [JsonPropertyName("message")]
    public string Message { get; }

    [JsonPropertyName("type")]
    public ResultErrorType Type { get; }
}
