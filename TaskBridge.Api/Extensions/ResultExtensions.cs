using Microsoft.AspNetCore.Mvc;

using TaskBridge.Application.Common;

namespace TaskBridge.Api.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value);

        var error = result.Error!;

        var problemDetails = new ProblemDetails
        {
            Title = GetTitle(error.Type),
            Detail = error.Message,
            Status = GetStatusCode(error.Type),
            Type = error.Code,
            Instance = controller.HttpContext.Request.Path`
        };

        problemDetails.Extensions["traceId"] = controller.HttpContext.TraceIdentifier;

        return controller.StatusCode(problemDetails.Status.Value, problemDetails);
    }

    private static int GetStatusCode(ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Failure => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string GetTitle(ErrorType type) => type switch
    {
        ErrorType.Validation => "Validation error",
        ErrorType.Unauthorized => "Unauthorized",
        ErrorType.Forbidden => "Forbidden",
        ErrorType.NotFound => "Not found",
        ErrorType.Conflict => "Conflict",
        ErrorType.Failure => "Failure",
        _ => "Error"
    };
}
