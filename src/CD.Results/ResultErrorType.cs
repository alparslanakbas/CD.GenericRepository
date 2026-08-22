using System.Text.Json.Serialization;

namespace CD.Results;

/// <summary>Classifies failures without coupling application code to a transport such as HTTP.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResultErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Unauthorized = 4,
    Forbidden = 5,
    Unexpected = 6,
}
