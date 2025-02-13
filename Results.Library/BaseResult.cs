using System.Net;
using System.Text.Json.Serialization;

namespace Results.Library
{
    public abstract class BaseResult
    {
        [JsonPropertyName("isSuccessful")]
        public bool IsSuccessful { get; protected set; }

        [JsonPropertyName("statusCode")]
        public HttpStatusCode StatusCode { get; protected set; }

        [JsonPropertyName("errorMessages")]
        public List<string>? ErrorMessages { get; protected set; }

        protected BaseResult() { }

        protected BaseResult(bool isSuccessful, HttpStatusCode statusCode)
        {
            IsSuccessful = isSuccessful;
            StatusCode = statusCode;
        }
    }
}
