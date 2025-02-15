using System.Net;
namespace CD.Results
{
    public static class ResultExtensions
    {
        public static async Task<Result<T>> ToResultAsync<T>(this Task<T> task)
        {
            try
            {
                var result = await task;
                return Result<T>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public static Result<T> ToResult<T>(this T value) => Result<T>.Success(value);
    }
}
