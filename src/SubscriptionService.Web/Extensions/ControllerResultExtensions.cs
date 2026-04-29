using Microsoft.AspNetCore.Mvc;
using SharedKernel.Result;
using SubscriptionService.Web.Contracts;

namespace SubscriptionService.Web.Extensions;

internal static class ControllerResultExtensions
{
    public static IActionResult FromErrorList(
        this ControllerBase controller,
        ErrorList errors)
    {
        ArgumentNullException.ThrowIfNull(controller);
        ArgumentNullException.ThrowIfNull(errors);

        var firstError = errors.FirstOrDefault();
        var statusCode = firstError is null
            ? StatusCodes.Status500InternalServerError
            : ToStatusCode(firstError.Type);

        return controller.StatusCode(
            statusCode,
            EndpointResult.Failure(errors, controller.HttpContext));
    }

    public static IActionResult FromResult(
        this ControllerBase controller,
        Result<Error> result,
        int successStatusCode = StatusCodes.Status200OK)
    {
        ArgumentNullException.ThrowIfNull(controller);
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess)
        {
            return successStatusCode switch
            {
                StatusCodes.Status204NoContent => controller.NoContent(),
                StatusCodes.Status201Created => controller.StatusCode(
                    StatusCodes.Status201Created,
                    EndpointResult.Success(controller.HttpContext)),
                _ => controller.Ok(EndpointResult.Success(controller.HttpContext))
            };
        }

        var error = result.Error ?? Error.Failure("Неизвестная ошибка.");
        var envelope = EndpointResult.Failure(error, controller.HttpContext);

        return controller.StatusCode(ToStatusCode(error.Type), envelope);
    }

    public static IActionResult FromResult<TValue>(
        this ControllerBase controller,
        Result<TValue, Error> result,
        int successStatusCode = StatusCodes.Status200OK,
        string? createdAtAction = null,
        object? routeValues = null)
    {
        ArgumentNullException.ThrowIfNull(controller);
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsFailure)
        {
            var error = result.Error ?? Error.Failure("Неизвестная ошибка.");
            return controller.FromResult(Result<Error>.Failure(error));
        }

        if (result.Value is null)
            return controller.Ok(EndpointResult.Success(controller.HttpContext));

        var envelope = EndpointResult.Success(result.Value, controller.HttpContext);

        if (createdAtAction is not null)
            return controller.CreatedAtAction(createdAtAction, routeValues, envelope);

        return successStatusCode == StatusCodes.Status201Created
            ? controller.StatusCode(StatusCodes.Status201Created, envelope)
            : controller.Ok(envelope);
    }

    private static int ToStatusCode(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Failure => StatusCodes.Status500InternalServerError,
            ErrorType.Null => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
}
