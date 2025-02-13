using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Results.Library
{
    public sealed class Result<T> : BaseResult
    {
        [JsonPropertyName("data")]
        public T? Data { get; private set; }

        private Result(T data) : base(true, HttpStatusCode.OK)
        {
            Data = data;
        }

        private Result(HttpStatusCode statusCode, List<string> errorMessages)
            : base(false, statusCode)
        {
            ErrorMessages = errorMessages;
        }

        private Result(HttpStatusCode statusCode, string errorMessage)
            : base(false, statusCode)
        {
            ErrorMessages = new() { errorMessage };
        }

        public static Result<T> Success(T data) => new(data);

        public static Result<T> Failure(HttpStatusCode statusCode, string errorMessage)
            => new(statusCode, errorMessage);

        public static Result<T> Failure(HttpStatusCode statusCode, List<string> errorMessages)
            => new(statusCode, errorMessages);

        public static Result<T> NotFound(string message = "Resource not found")
            => new(HttpStatusCode.NotFound, message);

        public static Result<T> BadRequest(string message)
            => new(HttpStatusCode.BadRequest, message);

        public static Result<T> Unauthorized(string message = "Unauthorized access")
            => new(HttpStatusCode.Unauthorized, message);

        public static Result<T> Forbidden(string message = "Forbidden access")
            => new(HttpStatusCode.Forbidden, message);

        public static implicit operator Result<T>(T data) => Success(data);

        public override string ToString()
            => JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
}
