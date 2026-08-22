using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CD.Results.AspNetCore;

/// <summary>Maps transport-independent results to ASP.NET Core responses.</summary>
public static class ResultHttpExtensions
{
    public static IResult ToHttpResult(this BaseResult result, int successStatusCode = StatusCodes.Status200OK)
    {
        ArgumentNullException.ThrowIfNull(result);
        int statusCode = GetStatusCode(result, successStatusCode);
        return statusCode == StatusCodes.Status204NoContent
            ? Microsoft.AspNetCore.Http.Results.NoContent()
            : Microsoft.AspNetCore.Http.Results.Json(result, statusCode: statusCode);
    }

    public static IActionResult ToActionResult(this BaseResult result, int successStatusCode = StatusCodes.Status200OK)
    {
        ArgumentNullException.ThrowIfNull(result);
        int statusCode = GetStatusCode(result, successStatusCode);
        return statusCode == StatusCodes.Status204NoContent
            ? new NoContentResult()
            : new ObjectResult(result) { StatusCode = statusCode };
    }

    public static int GetStatusCode(this BaseResult result, int successStatusCode = StatusCodes.Status200OK)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccessful)
        {
            if (successStatusCode is < 200 or > 299)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(successStatusCode),
                    successStatusCode,
                    "A successful result requires a 2xx HTTP status code.");
            }

            return successStatusCode;
        }

        return result.ErrorType switch
        {
            ResultErrorType.Validation or ResultErrorType.Failure => StatusCodes.Status400BadRequest,
            ResultErrorType.NotFound => StatusCodes.Status404NotFound,
            ResultErrorType.Conflict => StatusCodes.Status409Conflict,
            ResultErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ResultErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ResultErrorType.Unexpected => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError,
        };
    }
}
