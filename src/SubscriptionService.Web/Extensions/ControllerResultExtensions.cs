using Microsoft.AspNetCore.Mvc;
using SharedKernel.Result;
using SubscriptionService.Web.Contracts;

namespace SubscriptionService.Web.Extensions;

internal static class ControllerResultExtensions
{
    public static IActionResult FromResult(
        this ControllerBase controller,
        Result<Error> result)
    {
        ArgumentNullException.ThrowIfNull(controller);
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess)
            return controller.Ok(EndpointResult.Success(controller.HttpContext));

        var error = result.Error ?? Error.Failure("Неизвестная ошибка.");
        var envelope = EndpointResult.Failure(error, controller.HttpContext);

        return error.Type switch
        {
            ErrorType.Validation => controller.BadRequest(envelope),
            ErrorType.NotFound => controller.NotFound(envelope),
            ErrorType.Conflict => controller.Conflict(envelope),
            ErrorType.Forbidden => controller.StatusCode(StatusCodes.Status403Forbidden, envelope),
            ErrorType.Null => controller.BadRequest(envelope),
            _ => controller.StatusCode(StatusCodes.Status500InternalServerError, envelope)
        };
    }

    public static IActionResult FromResult<TValue>(
        this ControllerBase controller,
        Result<TValue, Error> result,
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

        return createdAtAction is null
            ? controller.Ok(envelope)
            : controller.CreatedAtAction(createdAtAction, routeValues, envelope);
    }
}
